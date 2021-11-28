using CommonVending.DbProvider;
using CommonVending.MqttModels;
using DeviceDbModel.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonVending.Crypt;
using CommonVending.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using MQTTnet.Client;

namespace CommonVending
{
  public static class DeviceMqtt
  {
    private static readonly IEmailSender sender = new AuthMessageSender();

    public static async Task SubscriptionHandler(string topic, string payLoad)
    {
      IMqttClient mqttClient = null;
      try
      {
        if (!topic.Contains("settings/todevice")) return;
        if (topic.Contains("cleaner")) return;
        if (topic.Contains("pay")) return;

        var tps = topic.Split("/");
        if (tps.Length < 4) return;

        var imei = tps[2];

        //check imei
        imei = new string(imei.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(imei) || (imei.Length < 15) || (imei.Length > 17)) return;

        var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(payLoad).WithExactlyOnceQoS().WithRetainFlag().Build();
        var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 8883)
            .WithClientId("device_settings")
            .WithCredentials("3voda", "Leimnoj8Knod")
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true, SslProtocol = System.Security.Authentication.SslProtocols.Tls12,

                //Certificates = certs,
                AllowUntrustedCertificates = true, IgnoreCertificateChainErrors = true, IgnoreCertificateRevocationErrors = true
            })
            .Build();

        // Create a new MQTT client.
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();
        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken

        //if (_mqttClient == null) await SubscriptionConnect();

        await mqttClient.PublishAsync(message, CancellationToken.None);
        await SendAesMessage(topic, payLoad);

        //Console.WriteLine("Message sent!");
      }
      catch (Exception ex) { Console.WriteLine(ex.Message); }
      finally
      {
        await mqttClient?.DisconnectAsync();
        mqttClient?.Dispose();
      }
    }

    private static async Task SendAesMessage(string topic, string payLoad)
    {
      IMqttClient mqttClient = null;
      try
      {
        var encrypt = CryptoAes.EncryptAes(payLoad);
        var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(encrypt).WithAtLeastOnceQoS().WithRetainFlag().Build();
        var options = new MqttClientOptionsBuilder().WithTcpServer("localhost", 1883).WithClientId("device_settings").Build();

        // Create a new MQTT client.
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();
        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        await mqttClient.PublishAsync(message, CancellationToken.None);
      }
      catch (Exception ex) { Console.WriteLine(ex.Message); }
      finally
      {
        await mqttClient?.DisconnectAsync();
        mqttClient?.Dispose();
      }
    }

    public static async Task MessageHandler(string topic, string message)
    {
      if (topic.Contains("settings/todevice")) return;
      if (topic.Contains("pay/response")) return;

      try
      {
        var tps = topic.Split("/");
        if (tps.Length < 4) return;

        var imei = tps[2];

        //check imei
        imei = new string(imei.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(imei) || (imei.Length < 15) || (imei.Length > 17)) return;

        var theme = tps[3];

        var device = DeviceDbProvider.GetDeviceByImei(imei);
        if (device == null)
        {
          Console.WriteLine($"[MQTT] Device [{imei}] not found!");
          return;

          //device = new Device
          //{
          //  Id = null, Address = "", Currency = "RUB", Imei = imei,
          //  Phone = "", TimeZone = 2,

          //  //OwnerId = ""
          //};
          //await DeviceDbProvider.AddOrEditDevice(device);
        }

        var lastState = StatusDbProvider.GetDeviceLastStatus(device.Id);

        //var lastAlert = DeviceDbProvider.GetDeviceLastAlert(device.Id);

        dynamic jdata = JObject.Parse(message);
        double timestamp = 0;
        if (jdata?.timestamp != null) timestamp = jdata.timestamp;
        if (timestamp == 0) timestamp = Main.ConvertToUnixTimestamp(DateTime.UtcNow);
        var msgDate = Main.ConvertFromUnixTimestamp(timestamp).AddHours(device.TimeZone);

        switch (theme)
        {
          case "status":
            // /status, Payload = {"timestamp":1621412974,"totalSold":5.000,"totalMoney":10.000,"temperature":9.760,"status":0}
            var status = JsonConvert.DeserializeObject<DevStatus>(message);
            status.Id = lastState?.Id ?? 0;
            status.DeviceId = device.Id;
            status.MessageDate = msgDate;

            StatusDbProvider.WriteDeviceLastStatus(status);

            // errors connect
            if (lastState != null)
            {
              var lastConnAlert = AlertsDbProvider.GetLastConnAlert(device.Id);

              if (lastState.MessageDate.AddMinutes(32) < msgDate)
              {
                if ((lastConnAlert == null) || (lastConnAlert.CodeType == 1))
                {
                  //error connect
                  // при появлении связи, просрочки во времени и последней аварии не "Пропала связь с автоматом" генерируем аварию
                  // со временем послед. + 10мин
                  var alert = new DevAlert {DeviceId = device.Id, MessageDate = lastState.MessageDate.AddMinutes(10), CodeType = -1, Message = "Пропала связь с автоматом"};
                  AlertsDbProvider.InsertDeviceAlert(alert);

                  // и сразу генерируем событие "Связь восстановилась"
                  alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 1, Message = "Связь восстановилась"};
                  AlertsDbProvider.InsertDeviceAlert(alert);
                }
                else if (lastConnAlert.CodeType == -1)
                {
                  var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 1, Message = "Связь восстановилась"};
                  AlertsDbProvider.InsertDeviceAlert(alert);
                }
              }

              //check sales
              if (lastState.TotalMoney == status.TotalMoney)
              {
                var lastSale = SalesDbProvider.GetLastSale(device.Id);
                if (lastSale != null)
                {
                  //not sales 2h
                  if (lastSale.MessageDate.AddHours(2) < msgDate)
                  {
                    var lastSlA = AlertsDbProvider.GetLastSaleAlert(device.Id);
                    if ((lastSlA == null) || (lastSlA.CodeType == 3))
                    {
                      //eror sales
                      var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 0, Message = "Нет продаж"};

                      AlertsDbProvider.InsertDeviceAlert(alert);

                      //send emails NO SALES
                      await sender.SendNoSales(device);
                    }
                  }
                }
              }
            }

            //tank
            if (status.Status == 1)
            {
              if ((lastState == null) || (lastState.Status != status.Status))
              {
                var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = -2, Message = "Бак пуст"};

                AlertsDbProvider.InsertDeviceAlert(alert);

                //send emails TANK EMPTY
                await sender.SendTankEmpty(device);
              }
            }
            else
            {
              if ((lastState != null) && (lastState.Status != status.Status))
              {
                var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 2, Message = "Бак заполняется"};
                AlertsDbProvider.InsertDeviceAlert(alert);
              }
            }

            break;

          case "encash":
            // /encash, Payload = {"timestamp":1621419926,"coins":[{"amount":12,"value":1}],"bills":[{"amount":1,"value":10}],"amountCoin":12,"amountBill":10,"amount":22}
            var newEncash = JsonConvert.DeserializeObject<MqttEncashModel>(message);
            var encash = new DevEncash
            {
                DeviceId = device.Id, MessageDate = msgDate, AmountCoin = newEncash.AmountCoin, AmountBill = newEncash.AmountBill,
                Amount = newEncash.Amount, Coins = JsonConvert.SerializeObject(newEncash.Coins), Bills = JsonConvert.SerializeObject(newEncash.Bills)
            };

            DeviceDbProvider.InsertDeviceEncash(encash);

            break;
          case "sales":
            var newSale = JsonConvert.DeserializeObject<MqttSaleModel>(message);
            var sale = new DevSale
            {
                DeviceId = device.Id, MessageDate = msgDate, PaymentType = newSale.PaymentType, Quantity = newSale.Quantity,
                Price = newSale.Price, Amount = newSale.Amount, Coins = JsonConvert.SerializeObject(newSale.Coins), Bills = JsonConvert.SerializeObject(newSale.Bills)
            };

            SalesDbProvider.InsertDeviceSale(sale);

            var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(device.Id);

            if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
            {
              var alert = new DevAlert {DeviceId = device.Id, MessageDate = sale.MessageDate, CodeType = 3, Message = "Продажи восстановились"};
              AlertsDbProvider.InsertDeviceAlert(alert);
            }

            break;
          case "info":
            var newInfo = JsonConvert.DeserializeObject<MqttInfoModel>(message);
            var info = new DevInfo {DeviceId = device.Id, MessageDate = msgDate, Name = newInfo.Name, Value = newInfo.Value};

            var lastInfo = DeviceDbProvider.GetDevInfo(info.DeviceId, info.Name);
            info.Id = lastInfo?.Id > 0 ? lastInfo.Id : 0;

            DeviceDbProvider.WriteDeviceLastInfo(info);

            /*info, Payload = {"timestamp":1621239421, "name":"signalStrength", "value":9.0}
              info, Payload = {"timestamp":1620134241, "name":"simBalance", "value":0.00}
              info, Payload = {"name":"energyT1","value":-1.0,"timestamp":1619682072}, 
              info, Payload = {"name":"energyT2","value":-1.0,"timestamp":1619682074}, 
              info, Payload = {"name":"waterInput","value":-1.0,"timestamp":1619682074}*/
            break;
          case "settings":
            const string tpc = "fromdevice";
            const int tpcType = 0;
            if (tps[4] != tpc) return;

            var sett = JsonConvert.DeserializeObject<MqttDeviceSettings>(message);
            sett.Timestamp = 0;
            var jsett = JsonConvert.SerializeObject(sett);
            var md5 = Main.GetMd5ByString(jsett);
            var lastSettings = DeviceDbProvider.GetLastSettings(device.Id, tpcType);
            var settings = new DevSetting
            {
                Id = lastSettings?.Id ?? 0, DeviceId = device.Id, MessageDate = msgDate, TopicType = tpcType,
                Topic = tpc, Payload = message, Md5 = md5
            };

            //if (lastSettings?.Md5 == settings.Md5) settings.Id = lastSettings.Id;

            DeviceDbProvider.WriteDeviceSettings(settings);
            break;
          case "cleaner":
            if (tps[4] != "status") return;

            const string tpcCleaner = "cleaner/status";
            const int tpcCleanerType = 1;
            var settClr = JsonConvert.DeserializeObject<MqttCleanerSettings>(message);
            settClr.Timestamp = 0;
            var jsettClr = JsonConvert.SerializeObject(settClr);
            var md5Clr = Main.GetMd5ByString(jsettClr);
            var lastCleanerSettings = DeviceDbProvider.GetLastSettings(device.Id, tpcCleanerType);
            var cleaner = new DevSetting
            {
                Id = lastCleanerSettings?.Id ?? 0, DeviceId = device.Id, MessageDate = msgDate, TopicType = tpcCleanerType,
                Topic = tpcCleaner, Payload = message, Md5 = md5Clr
            };

            //if (lastCleanerSettings?.Md5 == cleaner.Md5) cleaner.Id = lastCleanerSettings.Id;

            DeviceDbProvider.WriteDeviceSettings(cleaner);
            break;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        Console.WriteLine($"Topic - {topic} Message - {message}");
        if (topic.Contains("encash") || topic.Contains("sales"))
        {
          var error = new DevSetting
          {
              Id = 0, DeviceId = null, MessageDate = DateTime.UtcNow, TopicType = -1,
              Topic = topic, Payload = message, Md5 = null
          };
          DeviceDbProvider.WriteDeviceSettings(error);
        }
      }

      //finally
      //{
      //  if (topic.Contains("869640058515506"))
      //  {
      //    Console.WriteLine($"Topic - {topic} Message - {message}");
      //  }
      //}
    }
  }
}
