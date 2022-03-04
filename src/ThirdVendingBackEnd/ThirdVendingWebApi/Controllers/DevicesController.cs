using CommonVending;
using CommonVending.DbProvider;
using CommonVending.Model;
using CommonVending.MqttModels;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  /// <summary>
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class DevicesController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// </summary>
    /// <param name = "userManager"></param>
    public DevicesController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
      
      List<ApplicationUser> children;
      List<string> chId;
      switch (user.Role)
      {
        case Roles.SuperAdmin:
        case Roles.Admin:
          return new ObjectResult(DeviceDbProvider.GetAllDevices());
        case Roles.Dealer:
          children = UserDbProvider.GetUserDescendants(user.Id, true);
          chId = new List<string>();
          foreach (var ch in children)
          {
            chId.Add(ch.Id);
          }

          return new ObjectResult(DeviceDbProvider.GetAllDevices(chId));
        case Roles.DealerAdmin:
          children = UserDbProvider.GetUserDescendants(user.OwnerId, true);
          chId = new List<string>();
          foreach (var ch in children)
          {
            chId.Add(ch.Id);
          }

          return new ObjectResult(DeviceDbProvider.GetAllDevices(chId));
        case Roles.Owner:
          return new ObjectResult(DeviceDbProvider.GetAllDevices(new List<String> {user.Id}));
        case Roles.Technician:
          return new ObjectResult(DeviceDbProvider.GetAllDevices(new List<String> {user.OwnerId}, user.CommerceVisible));
      }

      return new ObjectResult(null);
    }

    [HttpPost("/api/device")]
    public async Task<IActionResult> Add([FromBody] DeviceAddModel dev)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var deviceOvnerId = user.Role == Roles.Technician ? user.OwnerId : user.Id;
      var device = new Device
      {
        Id = null, Address = dev.Address, Currency = dev.Currency, Imei = dev.Imei,
        Phone = dev.Phone, TimeZone = dev.TimeZone, OwnerId = deviceOvnerId
      };

      if (await DeviceDbProvider.AddOrEditDevice(device))
      {
        await UserDbProvider.AddLog(new LogUsr
        {
          Imei = device.Imei,
          UserId = user.Id,
          Email = user.Email,
          Phone = user.PhoneNumber,
          LogDate = DateTime.Now,
          Message = $"Пользователь {user} добавил автомат [{device.Imei}] - [{device.Address}]"
        });
        return Ok();
      }

      return BadRequest();
    }

    [HttpPut("/api/device/{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] DeviceAddModel dev)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var device = DeviceDbProvider.GetDeviceById(id);
      device.Imei = dev.Imei;
      device.Address = dev.Address;
      device.Currency = dev.Currency;
      device.Phone = dev.Phone;
      device.TimeZone = dev.TimeZone;

      if (await DeviceDbProvider.AddOrEditDevice(device))
      {
        await UserDbProvider.AddLog(new LogUsr
        {
          Imei = device.Imei,
          UserId = user.Id,
          Email = user.Email,
          Phone = user.PhoneNumber,
          LogDate = DateTime.Now,
          Message = $"Пользователь {user} изменил данные автомата [{device.Imei}] - [{device.Address}]"
        });
        return Ok();
      }

      return BadRequest();
    }

    /// <summary>
    ///   Удалить автомат
    /// </summary>
    /// <param name = "id"></param>
    /// <returns></returns>
    [HttpDelete("/api/device/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var device = DeviceDbProvider.GetDeviceById(id);
      if (device == null) return NotFound("Автомат не найден!");
      
      if (user.Role == Roles.Technician)
      {
        if (user.OwnerId != device.OwnerId) return StatusCode(403, "Запрещено удалять чужие автоматы!"); 
      }

      DeviceDbProvider.RemoveDevice(device);

      await UserDbProvider.AddLog(new LogUsr
      {
        Imei = device.Imei,
        UserId = user.Id,
        Email = user.Email,
        Phone = user.PhoneNumber,
        LogDate = DateTime.Now,
        Message = $"Пользователь {user} удалил автомат [{device.Imei}] - [{device.Address}]"
      });
      return Ok();
    }

    [HttpGet("/api/device/settings/{id}")]
    public async Task<IActionResult> GetSettings(string id)
    {
      return await Task.Factory.StartNew(() =>
      {
        var lastSettings = DeviceDbProvider.GetLastSettings(id, 0);
        if (lastSettings == null) return new ObjectResult(null);

        var settings = JsonConvert.DeserializeObject<MqttDeviceSettings>(lastSettings.Payload);
        return new ObjectResult(settings);
      });
    }

    [HttpPut("/api/device/settings/{imei}")]
    public async Task<IActionResult> SetSettings(string imei, [FromBody] PayLoadValue payLoad)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if (payLoad == null) return BadRequest("PayLoad is null!");

        object value = null;
        string vlstr = payLoad.Value.ToString();
        var ci = (CultureInfo) CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";

        var isNumbber = float.TryParse(vlstr, NumberStyles.Any, ci, out var val);
        
        switch (payLoad.Name)
        {
          case "pulsesPerLitre":
            value = new {pulsesPerLitre = (int) val};
            break;
          case "pricePerLitre":
            value = new {pricePerLitre = val};
            break;
          case "priceCard":
            value = new {priceCard = val};
            break;
          case "pulseValueCoin":
            value = new {pulseValueCoin = val};
            break;
          case "pulseValueBill":
            value = new {pulseValueBill = val};
            break;
          case "therm":
            value = new {therm = val};
            break;
          case "logo":
            value = new {logo = (int) val};
            break;
          case "date":
            value = new {date = (long) val / 1000};
            break;
          case "phone":
            value = new {phone = vlstr};
            break;
          case "maintain":
            value = new {maintain = (int) val};
            break;
        }

        var topic = $"3voda/device/{imei}/settings/todevice";
        await DeviceMqtt.SubscriptionHandler(topic, JsonConvert.SerializeObject(value, new JsonSerializerSettings {Formatting = Formatting.None}));
        await UserDbProvider.AddLog(new LogUsr
        {
          Imei = imei,
          UserId = user.Id,
          Email = user.Email,
          Phone = user.PhoneNumber,
          LogDate = DateTime.Now,
          Message = $"Пользователь {user} изменил настройки автомата [{imei}] [{payLoad.Name} = {payLoad.Value}]"
        });
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpGet("/api/device/info/{id}")]
    public async Task<IActionResult> GetInfo(string id)
    {
      return await Task.Factory.StartNew(() =>
      {
        try
        {
          var infos = DeviceDbProvider.GetDeviceInfos(id);
          var retList = infos.Select(Main.GetNewObj<DevInfoView>).ToList();

          var sortedList = new List<DevInfoView>();
          var elm = retList.Find(f => f.Name == "simBalance");
          if (elm != null) sortedList.Add(elm);

          elm = retList.Find(f => f.Name == "signalStrength");
          if (elm != null) sortedList.Add(elm);

          elm = retList.Find(f => f.Name == "energyT1");
          if (elm != null) sortedList.Add(elm);

          elm = retList.Find(f => f.Name == "energyT2");
          if (elm != null) sortedList.Add(elm);

          elm = retList.Find(f => f.Name == "waterInput");
          if (elm != null) sortedList.Add(elm);

          /* 
      new DeviceConstraints('simBalance', 'Баланс SIM', 20.0),
      new DeviceConstraints('signalStrength', 'Cигнал', 15, 20),
      new DeviceConstraints('energyT1', 'Тариф 1, КВт&middot;ч', 0.0),
      new DeviceConstraints('energyT2', 'Тариф 2, КВт&middot;ч', 0.0),
      new DeviceConstraints('waterInput', 'Водомер, м&sup3;', 0.0),
      new DeviceConstraints('tds', 'TDS, ppm', 0.0, 0.0, 100.0, 50.0),
      new DeviceConstraints('temperature', 'Температура, &deg;C', 0.0, 5.0, 50.0, 40.0)
           */
          return new ObjectResult(sortedList);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
          return StatusCode(500, $"Internal server error: {ex}");
        }
      });
    }

    [HttpPut("/api/device/setOwner/{id}")]
    public async Task<IActionResult> SetOwner(string id, [FromBody] string ownerId)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var user = await _userManager.FindByIdAsync(ownerId);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if (user.Role == Roles.Technician) return StatusCode(403, "Данные действия запрещены для техника!");
        if (user.Role != Roles.Owner) return NotFound("Пользователь не является владельцем!");

        await DeviceDbProvider.SetDeviceOwner(id, ownerId);
        var device = DeviceDbProvider.GetDeviceById(id);

        await UserDbProvider.AddLog(new LogUsr
        {
          Imei = device.Imei,
          UserId = admin.Id,
          Email = admin.Email,
          Phone = admin.PhoneNumber,
          LogDate = DateTime.Now,
          Message = $"Пользователь {admin} назначил владельца {user} автомату [{device.Imei}]"
        });
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    #region Nested types

    public class PayLoadValue
    {
      public string Name { get; set; }

      public dynamic Value { get; set; }
    }

    #endregion
  }
}
