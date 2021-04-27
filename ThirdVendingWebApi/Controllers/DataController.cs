using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataController : ControllerBase
  {
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
    public async Task<IActionResult> Send(long i, int tm, int ts, int te)
    {
      var imei = i.ToString();
      var device = DeviceDbProvider.GetDevice(imei);
      var result = true;
      if (device == null)
      {
        device = new Device
        {
          Id = Guid.NewGuid().ToString(), Address = "", Currency = "RUB", DeviceId = imei,
          Phone = "123456789", TimeZone = 2,

          //OwnerId = ""
        };
        result = await DeviceDbProvider.AddDevice(device);
      }

      //todo get last status сравнить данные и принять решение была ли продажа
      // можно сделать все на основании данных из status
      //todo get last alert сравнить данные и принять решение о записи ошибки
      //todo if tm=ts=0 get last encash сравнить данные и принять решение об инкассации
      Console.WriteLine($"imei={i} tm={tm} ts={ts} te={te}");
      if (result)
      {
        return Ok("OK");
      }

      return BadRequest();
    }
  }
}
