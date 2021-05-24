using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using CommonVending.MqttModels;
using DeviceDbModel.Models;
using Newtonsoft.Json;

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

        switch (theme)
        {
          case "status":
            var status = JsonConvert.DeserializeObject<DevStatus>(message);
            status.Id = lastState?.Id ?? 0;
            status.DeviceId = device.Id;
            status.ReceivedDate = date;
            status.MessageDate = Main.ConvertFromUnixTimestamp(status.Timestamp);

            DeviceDbProvider.WriteDeviceLastStatus(status);

            var msgDate = status.MessageDate;

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
                    DeviceId = device.Id, ReceivedDate = date, MessageDate = lastState.MessageDate.AddMinutes(12), CodeType = -1,
                    Message = "Пропала связь с автоматом"
                  };
                  DeviceDbProvider.InsertDeviceAlert(alert);

                  // и сразу генерим событие "Связь восстановилась"
                  alert = new DevAlert
                  {
                    DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 1,
                    Message = "Связь восстановилась"
                  };
                  DeviceDbProvider.InsertDeviceAlert(alert);
                }
                else if (lastConnAlert.CodeType == -1)
                {
                  var alert = new DevAlert
                  {
                    DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 1,
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
                        DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 0,
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
                  DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = -2,
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
                  DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 2,
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

            ///869395032100038/status, Payload = {"timestamp":1621412974,"totalSold":5.000,"totalMoney":10.000,"temperature":9.760,"status":0}
            break;

          case "encash":
            var encash = JsonConvert.DeserializeObject<DevEncash>(message);
            encash.DeviceId = device.Id;
            encash.ReceivedDate = date;
            encash.MessageDate = Main.ConvertFromUnixTimestamp(encash.Timestamp);
            DeviceDbProvider.InsertDeviceEncash(encash);

            //3voda/device/869395032100038/encash, Payload = {"timestamp":1621419926,"coins":[{"amount":12,"value":1}],"bills":[{"amount":1,"value":10}],"amountCoin":12,"amountBill":10,"amount":22}
            break;
          case "sales":
            var sale = JsonConvert.DeserializeObject<DevSale>(message);
            sale.DeviceId = device.Id;
            sale.ReceivedDate = date;
            sale.MessageDate = Main.ConvertFromUnixTimestamp(sale.Timestamp);
            DeviceDbProvider.InsertDeviceSale(sale);

            var lastSaleAlert = DeviceDbProvider.GetLastSaleAlert(device.Id);

            if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
            {
              var alert = new DevAlert
              {
                DeviceId = device.Id, ReceivedDate = date, MessageDate = sale.MessageDate, CodeType = 3,
                Message = "Продажи восстановились"
              };
              DeviceDbProvider.InsertDeviceAlert(alert);
            }

            break;
          case "info":
            var info = JsonConvert.DeserializeObject<DevInfo>(message);
            /*info, Payload = {"timestamp":1621239421, "name":"signalStrength", "value":9.0}
              info, Payload = {"timestamp":1620134241, "name":"simBalance", "value":0.00}
              info, Payload = {"name":"energyT1","value":-1.0,"timestamp":1619682072}, 
              info, Payload = {"name":"energyT2","value":-1.0,"timestamp":1619682074}, 
              info, Payload = {"name":"waterInput","value":-1.0,"timestamp":1619682074}
             
             */
            break;
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
