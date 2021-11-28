using CommonVending.DbProvider;
using CommonVending.MqttModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;
using ThirdVendingWebApi.Models.Device;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EncashController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public EncashController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      if ((user.Role == Roles.Technician) && !user.CommerceVisible) return new ObjectResult(null);

      //todo check role and permitions
      return await Task.Factory.StartNew(() =>
      {
        var encashes = (from != null) && (to != null)
                         ? DeviceDbProvider.GetDeviceEncash(deviceId, from.Value, to.Value)
                         : DeviceDbProvider.GetDeviceEncash(deviceId, 5);
        var retListEncashes = new List<DeviceEncashModel>();
        if (encashes == null) return new ObjectResult(null);
        
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
