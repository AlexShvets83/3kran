using CommonVending.DbProvider;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataController : ControllerBase
  {
    //public static StringBuilder Log { get; } = new StringBuilder();

    /*
     Протокол HTTP для старых автоматов
Сервис мониторинга получает от плат, работающих по стандарту HTTP, GET запрос в следующем виде:
i=86123412341234&tm=0&ts=0&te=0

где 	i – IMEI автомата
	tm – собрано денег
	ts – продано литров
	te – состояние автомата (0 – в работе, 1 – бак пуст)
В ответ при получении информации в корректном формате сервис отправляет символы «OK»
Сообщение передается при подаче питания на автомат, а далее при изменении статуса автомата (случившейся продаже, событии инкассации и пустом баке)
Плата не формирует события инкассации. Вместо этого она передает нулевые значения полей «tm» и «ts». Сервис мониторинга при ненулевой сумме продаж интерпретирует это как событие инкассации.
Полный путь GET запроса, с путем, с портом, с ресурсом в качестве примера
http://monitoring.3voda.ru/send?i=123456787654321&tm=1000&ts=500&te=0
         */
    [HttpGet("/send")]
    [AllowAnonymous]
    public async Task<IActionResult> Send(long i, float tm, float ts, int te)
    {
      var imei = i.ToString();
      if (string.IsNullOrEmpty(imei) || (imei.Length < 15) || (imei.Length > 17)) BadRequest("Неверный формат imei!");

      var device = DeviceDbProvider.GetDeviceByImei(imei);
      var result = true;
      if (device == null)
      {
        device = new Device
        {
          Id = Guid.NewGuid().ToString(), Address = "", Currency = "RUB", Imei = imei,
          Phone = "", TimeZone = 2,

          //OwnerId = ""
        };
        result = await DeviceDbProvider.AddDevice(device);
      }

      device = DeviceDbProvider.GetDeviceByImei(imei);
      var lastState = DeviceDbProvider.GetDeviceLastStatus(device.Id);
      //var lastAlert = DeviceDbProvider.GetDeviceLastAlert(device.Id);

      var date = DateTime.Now;
      var msgDate = date.ToUniversalTime().AddHours(device.TimeZone);
      var newState = new DevStatus
      {
        Id = lastState?.Id ?? 0, DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate,
        TotalMoney = tm, TotalSold = ts, Status = te
      };

      DeviceDbProvider.WriteDeviceLastStatus(newState);

      // errors connect
      if (lastState != null)
      {
        if (lastState.MessageDate.AddMinutes(12) < msgDate)
        {
          var lastConnAlert = DeviceDbProvider.GetLastConnAlert(device.Id);
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
          else if (lastConnAlert.CodeType == -1) //todo redundant case
          {
            var alert = new DevAlert
            {
              DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 1,
              Message = "Связь восстановилась"
            };
            DeviceDbProvider.InsertDeviceAlert(alert);
          }
        }
      }

      //tank
      if (te == 1)
      {
        if ((lastState == null) || (lastState.Status != te))
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
        if ((lastState != null) && (lastState.Status != te))
        {
          var alert = new DevAlert
          {
            DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 2,
            Message = "Бак заполняется"
          };
          DeviceDbProvider.InsertDeviceAlert(alert);
        }
      }

      //encash
      if ((tm == 0) && (ts == 0))
      {
        if ((lastState != null) && (lastState.TotalMoney > 0))
        {
          var encash = new DevEncash {DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, Amount = lastState.TotalMoney};
          DeviceDbProvider.InsertDeviceEncash(encash);
        }
      }
      else
      {
        var lastSaleAlert = DeviceDbProvider.GetLastSaleAlert(device.Id);
        
        //check sale
        if ((lastState == null) || (tm > lastState.TotalMoney) || (ts > lastState.TotalSold))
        {
          var quantity = ts - lastState?.TotalSold ?? ts;
          var amount = tm - lastState?.TotalMoney ?? tm;
          var sale = new DevSale
          {
            DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, PaymentType = 0,
            Amount = amount, Quantity = quantity
          };

          DeviceDbProvider.InsertDeviceSale(sale);

          if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
          {
            var alert = new DevAlert
            {
              DeviceId = device.Id, ReceivedDate = date, MessageDate = msgDate, CodeType = 3,
              Message = "Продажи восстановились"
            };
            DeviceDbProvider.InsertDeviceAlert(alert);
          }
        }
        else
        {
          var lastSale = DeviceDbProvider.GetLastSale(device.Id);
          if (lastSale != null)
          {
            //not sales 2h
            if (lastSale.MessageDate.AddHours(2) < msgDate)
            {
              if ((lastSaleAlert != null) && (lastSaleAlert.CodeType != 0))
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

      var ip = Request.HttpContext.Connection.RemoteIpAddress;

      //todo get last status сравнить данные и принять решение была ли продажа
      // можно сделать все на основании данных из status
      //todo get last alert сравнить данные и принять решение о записи ошибки
      //todo if tm=ts=0 get last encash сравнить данные и принять решение об инкассации
      //Log.AppendLine($"{DateTime.Now} imei={i} tm={tm} ts={ts} te={te}");
      Console.WriteLine($"ip={ip} imei={i} tm={tm} ts={ts} te={te}");
      if (result) { return Ok("OK"); }

      return BadRequest();
    }
  }
}
