using CommonVending;
using CommonVending.DbProvider;
using CommonVending.Services;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataController : ControllerBase
  {
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "emailSender"></param>
    public DataController(IEmailSender emailSender)
    {
      _emailSender = emailSender;
    }

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
      if (!ApplicationSettings.SupportBoard1) return NotFound();

      var imei = i.ToString();
      if (string.IsNullOrEmpty(imei) || (imei.Length < 15) || (imei.Length > 17)) BadRequest("Неверный формат imei!");

      //var result = true;
      var device = DeviceDbProvider.GetDeviceByImei(imei);
      if (device == null)
      {
        Console.WriteLine($"Device [{imei}] not found!");
        return NotFound();

        //device = new Device
        //{
        //  Id = null, Address = "", Currency = "RUB", Imei = imei,
        //  Phone = "", TimeZone = 2,

        //  //OwnerId = ""
        //};
        //result = await DeviceDbProvider.AddOrEditDevice(device);
      }
      
      var lastState = StatusDbProvider.GetDeviceLastStatus(device.Id);

      //var lastAlert = AlertsDbProvider.GetDeviceLastAlert(device.Id);
      var msgDate = DateTime.UtcNow.AddHours(device.TimeZone);

      var newState = new DevStatus
      {
        Id = lastState?.Id ?? 0, DeviceId = device.Id, MessageDate = msgDate, TotalMoney = tm,
        TotalSold = ts, Status = te
      };

      StatusDbProvider.WriteDeviceLastStatus(newState);

      // errors connect
      if (lastState != null)
      {
        if (lastState.MessageDate.AddMinutes(32) < msgDate)
        {
          var lastConnAlert = AlertsDbProvider.GetLastConnAlert(device.Id);
          if ((lastConnAlert == null) || (lastConnAlert.CodeType == 1))
          {
            //error connect
            // при появлении связи, просрочки во времени и последней аварии не "Пропала связь с автоматом" генерим аварию
            // со временем послед. + 10мин
            var alert = new DevAlert {DeviceId = device.Id, MessageDate = lastState.MessageDate.AddMinutes(10), CodeType = -1, Message = "Пропала связь с автоматом"};
            AlertsDbProvider.InsertDeviceAlert(alert);

            // и сразу генерим событие "Связь восстановилась"
            alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 1, Message = "Связь восстановилась"};
            AlertsDbProvider.InsertDeviceAlert(alert);
          }
          else if (lastConnAlert.CodeType == -1) //todo maybe redundant case
          {
            var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 1, Message = "Связь восстановилась"};
            AlertsDbProvider.InsertDeviceAlert(alert);
          }
        }
      }

      //tank
      if (te == 1)
      {
        if ((lastState == null) || (lastState.Status != te))
        {
          var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = -2, Message = "Бак пуст"};
          AlertsDbProvider.InsertDeviceAlert(alert);
          await _emailSender.SendTankEmpty(device);
        }
      }
      else
      {
        if ((lastState != null) && (lastState.Status != te))
        {
          var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 2, Message = "Бак заполняется"};
          AlertsDbProvider.InsertDeviceAlert(alert);
        }
      }

      //encash
      if ((tm == 0) && (ts == 0))
      {
        if ((lastState != null) && (lastState.TotalMoney > 0))
        {
          var encash = new DevEncash {DeviceId = device.Id, MessageDate = msgDate, Amount = lastState.TotalMoney};
          DeviceDbProvider.InsertDeviceEncash(encash);
        }
      }

      //else
      //{
      //  var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(device.Id);

      //  //check sale
      //  if ((lastState == null) || ((tm > lastState.TotalMoney) && (ts > lastState.TotalSold)))
      //  {
      //    //todo if quantity != 0 || amount != 0 - it's sale? maybe negative amount

      //    var quantity = ts - lastState?.TotalSold ?? ts;
      //    var amount = tm - lastState?.TotalMoney ?? tm;
      //    var sale = new DevSale
      //    {
      //      DeviceId = device.Id, /*ReceivedDate = date,*/ MessageDate = msgDate, PaymentType = 0,
      //      Amount = amount, Quantity = quantity
      //    };

      //    DeviceDbProvider.InsertDeviceSale(sale);

      //    if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
      //    {
      //      var alert = new DevAlert
      //      {
      //        DeviceId = device.Id, /*ReceivedDate = date, */MessageDate = msgDate, CodeType = 3,
      //        Message = "Продажи восстановились"
      //      };
      //      AlertsDbProvider.InsertDeviceAlert(alert);
      //    }
      //  }
      //  else
      //  {
      //    var lastSale = DeviceDbProvider.GetLastSale(device.Id);
      //    if (lastSale != null)
      //    {
      //      //not sales 2h
      //      if (lastSale.MessageDate.AddHours(2) < msgDate)
      //      {
      //        if ((lastSaleAlert != null) && (lastSaleAlert.CodeType != 0))
      //        {
      //          //eror sales
      //          var alert = new DevAlert
      //          {
      //            DeviceId = device.Id,/* ReceivedDate = date,*/ MessageDate = msgDate, CodeType = 0,
      //            Message = "Нет продаж"
      //          };
      //          AlertsDbProvider.InsertDeviceAlert(alert);
      //        }
      //      }
      //    }
      //  }
      //}

      //check sale
      else if (lastState != null)
      {
        if ((tm > lastState.TotalMoney) && (ts > lastState.TotalSold))
        {
          // it's sale
          var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(device.Id);
          var quantity = ts - lastState.TotalSold;
          var amount = tm - lastState.TotalMoney;
          var sale = new DevSale
          {
            DeviceId = device.Id, MessageDate = msgDate, PaymentType = 0, Amount = amount,
            Quantity = quantity
          };

          SalesDbProvider.InsertDeviceSale(sale);

          if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
          {
            var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 3, Message = "Продажи восстановились"};
            AlertsDbProvider.InsertDeviceAlert(alert);
          }
        }
        else if ((tm != lastState.TotalMoney) || (ts != lastState.TotalSold))
        {
          var errorState = new DevErrorStatus
          {
            Id = 0, DeviceId = device.Id, MessageDate = msgDate, TotalMoney = tm,
            TotalSold = ts, Status = te
          };

          StatusDbProvider.WriteDeviceErrorStatus(errorState);

          // todo error to anothe table
          lastState.Status = te;
          lastState.MessageDate = msgDate;
          StatusDbProvider.WriteDeviceLastStatus(lastState);
        }
        else
        {
          if (CheckSaleError(device.Id, msgDate)) await _emailSender.SendNoSales(device);
        }
      }
      else //if ((tm != 0) || (ts != 0))
      {
        if (CheckSaleError(device.Id, msgDate)) await _emailSender.SendNoSales(device);

        //// mybe it's FIRST sale
        //var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(device.Id);
        //var quantity = ts;
        //var amount = tm;
        //var sale = new DevSale
        //{
        //  DeviceId = device.Id, MessageDate = msgDate, PaymentType = 0, Amount = amount,
        //  Quantity = quantity
        //};

        //SalesDbProvider.InsertDeviceSale(sale);

        //if ((lastSaleAlert != null) && (lastSaleAlert.CodeType == 0))
        //{
        //  var alert = new DevAlert {DeviceId = device.Id, MessageDate = msgDate, CodeType = 3, Message = "Продажи восстановились"};
        //  AlertsDbProvider.InsertDeviceAlert(alert);
        //}
      }
      
      //if (i == 869640058515506)
      //{
      //  var ip = Request.HttpContext.Connection.RemoteIpAddress;
      //  Console.WriteLine($"ip={ip} imei={i} tm={tm} ts={ts} te={te}");
      //}

      //Console.WriteLine($"ip={ip} imei={i} tm={tm} ts={ts} te={te}");
     
      return Ok("OK");

      //if (result) { return Ok("OK"); }
      //return BadRequest();
    }

    private bool CheckSaleError(string dviceId, DateTime msgDate)
    {
      var lastSale = SalesDbProvider.GetLastSale(dviceId);
      if (lastSale != null)
      {
        //not sales 2h
        if (lastSale.MessageDate.AddHours(2) < msgDate)
        {
          var lastSaleAlert = AlertsDbProvider.GetLastSaleAlert(dviceId);

          if ((lastSaleAlert != null) && (lastSaleAlert.CodeType != 0))
          {
            //eror sales
            var alert = new DevAlert {DeviceId = dviceId, MessageDate = msgDate, CodeType = 0, Message = "Нет продаж"};
            AlertsDbProvider.InsertDeviceAlert(alert);
            return true;
          }
        }
      }

      return false;
    }

    [HttpGet("/unixtimestamp")]
    [AllowAnonymous]
    public IActionResult GetTimestamp()
    {
      var dateNow = DateTime.UtcNow;
      var timestamp = Main.ConvertToUnixTimestamp(dateNow);
      var ret = new {timestamp};
      return Ok(ret);
    }
  }
}
