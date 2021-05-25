using CommonVending.DbProvider;
using CommonVending.MqttModels;
using DeviceDbModel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace CommonVending
{
  public class DeviceMqtt
  {
    public static void MessageHandler(string topic, string message)
    {
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
          var result = DeviceDbProvider.AddDevice(device).Result;
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
            status.ReceivedDate = date;
            status.MessageDate = msgDate;

            DeviceDbProvider.WriteDeviceLastStatus(status);

            //var msgDate = status.MessageDate;

            // errors connect
            if (lastState != null)
            {
              var lastConnAlert = DeviceDbProvider.GetLastConnAlert(device.Id);

              if (lastState.MessageDate.AddMinutes(12) < msgDate)
              {
                if ((lastConnAlert == null) || (lastConnAlert.CodeType == 1))
                {
                  //error connect
                  // при появлении связи, просрочки во времени и последней аварии не "Пропала связь с автоматом" генерим аварию
                  // со временем послед. + 10мин
                  var alert = new DevAlert
                  {
                    DeviceId = device.Id,
                    ReceivedDate = date,
                    MessageDate = lastState.MessageDate.AddMinutes(10),
                    Timestamp = status.Timestamp,
                    CodeType = -1,
                    Message = "Пропала связь с автоматом"
                  };
                  DeviceDbProvider.InsertDeviceAlert(alert);

                  // и сразу генерим событие "Связь восстановилась"
                  alert = new DevAlert
                  {
                    DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
                    Timestamp = status.Timestamp,
                    CodeType = 1,
                    Message = "Связь восстановилась"
                  };
                  DeviceDbProvider.InsertDeviceAlert(alert);
                }
                else if (lastConnAlert.CodeType == -1)
                {
                  var alert = new DevAlert
                  {
                    DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
                    Timestamp = status.Timestamp, CodeType = 1,
                    Message = "Связь восстановилась"
                  };
                  DeviceDbProvider.InsertDeviceAlert(alert);
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
                    var lastSlA = DeviceDbProvider.GetLastSaleAlert(device.Id);
                    if ((lastSlA == null) || (lastSlA.CodeType == 3))
                    {
                      //eror sales
                      var alert = new DevAlert
                      {
                        DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
                        Timestamp = status.Timestamp,
                        CodeType = 0,
                        Message = "Нет продаж"
                      };
                      DeviceDbProvider.InsertDeviceAlert(alert);
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
                  DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
                  Timestamp = status.Timestamp, CodeType = -2,
                  Message = "Бак пуст"
                };
                DeviceDbProvider.InsertDeviceAlert(alert);
              }
            }
            else
            {
              if ((lastState != null) && (lastState.Status != status.Status))
              {
                var alert = new DevAlert
                {
                  DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
                  Timestamp = status.Timestamp, CodeType = 2,
                  Message = "Бак заполняется"
                };
                DeviceDbProvider.InsertDeviceAlert(alert);
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
              DeviceId = device.Id,
              ReceivedDate = date,
              MessageDate = msgDate,
              Timestamp = newEncash.Timestamp,
              AmountCoin = newEncash.AmountCoin,
              AmountBill = newEncash.AmountBill,
              Amount = newEncash.Amount,
              Coins = JsonConvert.SerializeObject(newEncash.Coins),
              Bills = JsonConvert.SerializeObject(newEncash.Bills)
            };

            DeviceDbProvider.InsertDeviceEncash(encash);

            //3voda/device/869395032100038/encash, Payload = {"timestamp":1621419926,"coins":[{"amount":12,"value":1}],"bills":[{"amount":1,"value":10}],"amountCoin":12,"amountBill":10,"amount":22}
            break;
          case "sales":
            var newSale = JsonConvert.DeserializeObject<MqttSaleModel>(message);
            var sale = new DevSale
            {
              DeviceId = device.Id,
              ReceivedDate = date,
              MessageDate = msgDate,
              Timestamp = newSale.Timestamp,
              PaymentType = newSale.PaymentType,
              Quantity = newSale.Quantity,
              Price = newSale.Price,
              Amount = newSale.Amount,
              Coins = JsonConvert.SerializeObject(newSale.Coins),
              Bills = JsonConvert.SerializeObject(newSale.Bills)
            };

            DeviceDbProvider.InsertDeviceSale(sale);

            var lastSaleAlert = DeviceDbProvider.GetLastSaleAlert(device.Id);

            if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
            {
              var alert = new DevAlert
              {
                DeviceId = device.Id, ReceivedDate = date, MessageDate = sale.MessageDate, 
                Timestamp = newSale.Timestamp, CodeType = 3,
                Message = "Продажи восстановились"
              };
              DeviceDbProvider.InsertDeviceAlert(alert);
            }

            break;
          case "info":
            var newInfo = JsonConvert.DeserializeObject<MqttInfoModel>(message);
            var info = new DevInfo
            {
              DeviceId = device.Id,
              ReceivedDate = date,
              MessageDate = msgDate,
              Name = newInfo.Name,
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
            //dynamic data = JObject.Parse(message);
            //double timestamp = data.timestamp;
            const string tpc = "fromdevice";
            var sett = JsonConvert.DeserializeObject<MqttDeviceSettings>(message);
            sett.Timestamp = 0;
            var jsett = JsonConvert.SerializeObject(sett);
            var md5 = Main.GetMd5ByCMOS(jsett);
            var settings = new DevSetting
            {
              DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, Topic = tpc,
              Payload = message, Md5 = md5
            };
            var lastSettings = DeviceDbProvider.GetLastSettings(device.Id, tpc);
            if (lastSettings?.Md5 == settings.Md5) settings.Id = lastSettings.Id;

            DeviceDbProvider.WriteDeviceSettings(settings);
            break;
          case "cleaner":
            const string tpcCleaner = "cleaner/fromdevice";
            var settClr = JsonConvert.DeserializeObject<MqttCleanerSettings>(message);
            settClr.Timestamp = 0;
            var jsettClr = JsonConvert.SerializeObject(settClr);
            var md5Clr = Main.GetMd5ByCMOS(jsettClr);
            var cleaner = new DevSetting
            {
              DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, Topic = tpcCleaner,
              Payload = message, Md5 = md5Clr
            };
            var lastCleanerSettings = DeviceDbProvider.GetLastSettings(device.Id, tpcCleaner);
            if (lastCleanerSettings?.Md5 == cleaner.Md5) cleaner.Id = lastCleanerSettings.Id;
            break;
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
