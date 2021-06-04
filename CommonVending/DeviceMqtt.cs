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

namespace CommonVending
{
  public class DeviceMqtt
  {
    public static async Task SubscriptionHandler(string topic)
    {
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

        var theme = tps[3];

        var device = DeviceDbProvider.GetDeviceByImei(imei);
        if (device == null)
        {
          device = new Device
          {
            Id = Guid.NewGuid().ToString(), Address = "", Currency = "RUB", Imei = imei,
            Phone = "", TimeZone = 2,

            //OwnerId = ""
          };
          await DeviceDbProvider.AddDevice(device);
        }

        device = DeviceDbProvider.GetDeviceByImei(imei);

        //topic = "3voda/device/869244046767509/settings/todevice";
        //var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ////var currentPath = "/etc/letsencrypt/live/monitoring3voda.ru/";
        //var certificate = new X509Certificate2(Path.Combine(currentPath ?? string.Empty, "certificate.pfx"), "qwerty", X509KeyStorageFlags.Exportable);
        

//        var pathCert = "/etc/letsencrypt/live/monitoring3voda.ru/fullchain.pem";
//        var pathKey = "/etc/letsencrypt/live/monitoring3voda.ru/privkey.pem";

//#if DEBUG
//        pathCert = "d:\\!Projects\\!3Cran\\SSL\\monitoring3voda.ru\\cert.pem";
//        pathKey = "d:\\!Projects\\!3Cran\\SSL\\monitoring3voda.ru\\privkey.pem";
//#endif
//        var fc = await File.ReadAllTextAsync(pathCert);
//        var fk = await File.ReadAllTextAsync(pathKey);

//        var certificate = X509Certificate2.CreateFromPem(fc, fk);
        
//        List<X509Certificate> certs = new List<X509Certificate>
//        {
//          certificate
//        };

        var message = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(@"{""pricePerLitre"":3.000}").WithExactlyOnceQoS().WithRetainFlag().Build();
        var options = new MqttClientOptionsBuilder()
          
          //.WithTcpServer("127.0.0.1", 8883)
          .WithTcpServer("localhost", 8883)
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
        var mqttClient = factory.CreateMqttClient();
        await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        await mqttClient.PublishAsync(message, CancellationToken.None);
        Console.WriteLine("Message send!");
      }
      catch (Exception ex) { Console.WriteLine(ex.Message); }
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
          device = new Device
          {
            Id = Guid.NewGuid().ToString(), Address = "", Currency = "RUB", Imei = imei,
            Phone = "", TimeZone = 2,

            //OwnerId = ""
          };
          await DeviceDbProvider.AddDevice(device);
        }

        device = DeviceDbProvider.GetDeviceByImei(imei);
        var lastState = DeviceDbProvider.GetDeviceLastStatus(device.Id);

        //var lastAlert = DeviceDbProvider.GetDeviceLastAlert(device.Id);

        var date = DateTime.Now;
        dynamic jdata = JObject.Parse(message);
        double timestamp = jdata.timestamp;
        var msgDate = Main.ConvertFromUnixTimestamp(timestamp).AddHours(device.TimeZone);

        switch (theme)
        {
          case "status":
            var status = JsonConvert.DeserializeObject<DevStatus>(message);
            status.Id = lastState?.Id ?? 0;
            status.DeviceId = device.Id;
            //status.ReceivedDate = date;
            status.MessageDate = msgDate;

            DeviceDbProvider.WriteDeviceLastStatus(status);

            //var msgDate = status.MessageDate;

            // errors connect
            if (lastState != null)
            {
              var lastConnAlert = AlertsDbProvider.GetLastConnAlert(device.Id);

              if (lastState.MessageDate.AddMinutes(32) < msgDate)
              {
                if ((lastConnAlert == null) || (lastConnAlert.CodeType == 1))
                {
                  //error connect
                  // при появлении связи, просрочки во времени и последней аварии не "Пропала связь с автоматом" генерим аварию
                  // со временем послед. + 10мин
                  var alert = new DevAlert
                  {
                    DeviceId = device.Id, //ReceivedDate = date, 
                    MessageDate = lastState.MessageDate.AddMinutes(10), /*Timestamp = status.Timestamp,*/
                    CodeType = -1, Message = "Пропала связь с автоматом"
                  };
                  AlertsDbProvider.InsertDeviceAlert(alert);

                  // и сразу генерим событие "Связь восстановилась"
                  alert = new DevAlert
                  {
                    DeviceId = device.Id, //ReceivedDate = date, 
                    MessageDate = msgDate,/* Timestamp = status.Timestamp,*/
                    CodeType = 1, Message = "Связь восстановилась"
                  };
                  AlertsDbProvider.InsertDeviceAlert(alert);
                }
                else if (lastConnAlert.CodeType == -1)
                {
                  var alert = new DevAlert
                  {
                    DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, /*Timestamp = status.Timestamp,*/
                    CodeType = 1, Message = "Связь восстановилась"
                  };
                  AlertsDbProvider.InsertDeviceAlert(alert);
                }
              }

              //check sales
              if (lastState.TotalMoney == status.TotalMoney)
              {
                var lastSale = DeviceDbProvider.GetLastSale(device.Id);
                if (lastSale != null)
                {
                  //not sales 2h
                  if (lastSale.MessageDate.AddHours(2) < msgDate)
                  {
                    var lastSlA = AlertsDbProvider.GetLastSaleAlert(device.Id);
                    if ((lastSlA == null) || (lastSlA.CodeType == 3))
                    {
                      //eror sales
                      var alert = new DevAlert
                      {
                        DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, /*Timestamp = status.Timestamp,*/
                        CodeType = 0, Message = "Нет продаж"
                      };
                      AlertsDbProvider.InsertDeviceAlert(alert);
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
                var alert = new DevAlert
                {
                  DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, /*Timestamp = status.Timestamp,*/
                  CodeType = -2, Message = "Бак пуст"
                };
                AlertsDbProvider.InsertDeviceAlert(alert);
              }
            }
            else
            {
              if ((lastState != null) && (lastState.Status != status.Status))
              {
                var alert = new DevAlert
                {
                  DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate,/* Timestamp = status.Timestamp,*/
                  CodeType = 2, Message = "Бак заполняется"
                };
                AlertsDbProvider.InsertDeviceAlert(alert);
              }
            }

            //var newState = new DevStatus
            //{
            //  Id = lastState?.Id ?? 0, DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
            //  TotalMoney = status.TotalMoney, TotalSold = status.TotalSold, Status = status.Status
            //};
            //869395032100038/status, Payload = {"timestamp":1621412974,"totalSold":5.000,"totalMoney":10.000,"temperature":9.760,"status":0}
            break;

          case "encash":
            var newEncash = JsonConvert.DeserializeObject<MqttEncashModel>(message);
            var encash = new DevEncash
            {
              DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, /*Timestamp = newEncash.Timestamp,*/
              AmountCoin = newEncash.AmountCoin, AmountBill = newEncash.AmountBill, Amount = newEncash.Amount, Coins = JsonConvert.SerializeObject(newEncash.Coins),
              Bills = JsonConvert.SerializeObject(newEncash.Bills)
            };

            DeviceDbProvider.InsertDeviceEncash(encash);

            //3voda/device/869395032100038/encash, Payload = {"timestamp":1621419926,"coins":[{"amount":12,"value":1}],"bills":[{"amount":1,"value":10}],"amountCoin":12,"amountBill":10,"amount":22}
            break;
          case "sales":
            var newSale = JsonConvert.DeserializeObject<MqttSaleModel>(message);
            var sale = new DevSale
            {
              DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, /*Timestamp = newSale.Timestamp,*/
              PaymentType = newSale.PaymentType, Quantity = newSale.Quantity, Price = newSale.Price, Amount = newSale.Amount,
              Coins = JsonConvert.SerializeObject(newSale.Coins), Bills = JsonConvert.SerializeObject(newSale.Bills)
            };

            DeviceDbProvider.InsertDeviceSale(sale);

            var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(device.Id);

            if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
            {
              var alert = new DevAlert
              {
                DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = sale.MessageDate, /*Timestamp = newSale.Timestamp,*/
                CodeType = 3, Message = "Продажи восстановились"
              };
              AlertsDbProvider.InsertDeviceAlert(alert);
            }

            break;
          case "info":
            var newInfo = JsonConvert.DeserializeObject<MqttInfoModel>(message);
            var info = new DevInfo
            {
              DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, Name = newInfo.Name,
              Value = newInfo.Value
            };

            var lastInfo = DeviceDbProvider.GetDevInfo(info.DeviceId, info.Name);
            info.Id = lastInfo?.Id > 0 ? lastInfo.Id : 0;

            DeviceDbProvider.WriteDeviceLastInfo(info);

            //info.Timestamp = newSale.Timestamp,
            /*info, Payload = {"timestamp":1621239421, "name":"signalStrength", "value":9.0}
              info, Payload = {"timestamp":1620134241, "name":"simBalance", "value":0.00}
              info, Payload = {"name":"energyT1","value":-1.0,"timestamp":1619682072}, 
              info, Payload = {"name":"energyT2","value":-1.0,"timestamp":1619682074}, 
              info, Payload = {"name":"waterInput","value":-1.0,"timestamp":1619682074}
             
             */
            break;
          case "settings":
            const string tpc = "fromdevice";
            const int tpcType = 0;
            if (tps[4] != tpc) return;

            //dynamic data = JObject.Parse(message);
            //double timestamp = data.timestamp;
            var sett = JsonConvert.DeserializeObject<MqttDeviceSettings>(message);
            sett.Timestamp = 0;
            var jsett = JsonConvert.SerializeObject(sett);
            var md5 = Main.GetMd5ByCMOS(jsett);
            var settings = new DevSetting
            {
              DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, TopicType = tpcType,
              Topic = tpc, Payload = message, Md5 = md5
            };
            var lastSettings = DeviceDbProvider.GetLastSettings(device.Id, tpcType);
            if (lastSettings?.Md5 == settings.Md5) settings.Id = lastSettings.Id;

            DeviceDbProvider.WriteDeviceSettings(settings);
            break;
          case "cleaner":
            if (tps[5] != "status") return;

            const string tpcCleaner = "cleaner/status";
            const int tpcCleanerType = 1;
            var settClr = JsonConvert.DeserializeObject<MqttCleanerSettings>(message);
            settClr.Timestamp = 0;
            var jsettClr = JsonConvert.SerializeObject(settClr);
            var md5Clr = Main.GetMd5ByCMOS(jsettClr);
            var cleaner = new DevSetting
            {
              DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, TopicType = tpcCleanerType,
              Topic = tpcCleaner, Payload = message, Md5 = md5Clr
            };
            var lastCleanerSettings = DeviceDbProvider.GetLastSettings(device.Id, tpcCleanerType);
            if (lastCleanerSettings?.Md5 == cleaner.Md5) cleaner.Id = lastCleanerSettings.Id;
            break;
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
