using CommonVending;
using CommonVending.DbProvider;
using CommonVending.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.MqttModels;
using Newtonsoft.Json;
using ThirdVendingWebApi.Models.Device;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EncashController : ControllerBase
  {
    [HttpGet]

    //[Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      return await Task.Factory.StartNew(() =>
      {
        var encashes = (from != null) && (to != null)
                         ? DeviceDbProvider.GetDeviceEncash(deviceId, from.Value, to.Value)
                         : DeviceDbProvider.GetDeviceEncash(deviceId, 5);
        var retListEncashes = new List<DeviceEncashModel>();
        foreach (var encashe in encashes)
        {
          var enc = new DeviceEncashModel
          {
            MessageDate = encashe.MessageDate,
            Amount = encashe.Amount,
            Coins = encashe.Coins != null ? JsonConvert.DeserializeObject<MqttMoney[]>(encashe.Coins) : null,
            Bills = encashe.Bills != null ? JsonConvert.DeserializeObject<MqttMoney[]>(encashe.Bills) : null
          };

          enc.AmountCoin = SalesController.SummMoney(enc.Coins);
          enc.AmountBill = SalesController.SummMoney(enc.Bills);

          retListEncashes.Add(enc);
        }
        //var retList = encashes.Select(Main.GetNewObj<DevEncashView>).ToList();
        return new ObjectResult(retListEncashes);
      });
    }
  }
}
