using CommonVending;
using CommonVending.DbProvider;
using CommonVending.Model;
using CommonVending.MqttModels;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
  public class DevicesController : ControllerBase
  {
    //private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    //private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// </summary>
    /// <param name = "userManager"></param>
    public DevicesController(UserManager<ApplicationUser> userManager) //, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      //_signInManager = signInManager;
      //_roleManager = roleManager;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
      //Stopwatch st = new Stopwatch();
      //st.Start();
      //var devList = DeviceDbProvider.GetAllDevices();
      
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      List<DevView> devList = null;
      List<string> chId = null;
      List<ApplicationUser> children = null;

      //var userRoles = await _userManager.GetRolesAsync(user);
      //var role = userRoles[0];
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
          return new ObjectResult(DeviceDbProvider.GetAllDevices(new List<String>{user.Id}));
        case Roles.Technician:
          return new ObjectResult(DeviceDbProvider.GetAllDevices(new List<String>{user.OwnerId}, user.CommerceVisible));
      }

      //st.Stop();
      //Console.WriteLine(st.Elapsed);


      //var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
      //Response.Headers.Add("Link", headerStr);
      //Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(devList);
    }

    //[HttpGet("/api/_search/devices")]
    //[Authorize]
    //public async Task<IActionResult> GetDevices(string query, int page, int size, string sort) //Get(int size)
    //{
    //  //var user = await _userManager.GetUserAsync(HttpContext.User);
    //  //if (user == null) return NotFound();
    //  //if (!user.Activated) return NotFound();
    //  //http: //95.183.10.198/api/_search/devices?query=f2f196db-4cb1-47a4-9b74-2f602c2c5517&page=0&size=10&sort=id.keyword,desc
    //  var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
    //  Response.Headers.Add("Link", headerStr);
    //  Response.Headers.Add("X-Total-Count", "0");
    //  return new ObjectResult(new string [0]);
    //}

    [HttpPost("/api/device")]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] DeviceAddModel dev)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      //var userRoles = await _userManager.GetRolesAsync(user);
      var ovnerID = user.Id;
      if (user.Role == Roles.Technician) { ovnerID = user.OwnerId; }

      //if (string.IsNullOrEmpty(ovnerID))
      //{
      //  //ovnerID = userRoles.Contains(Roles.Owner) ? user.Id : DeviceTool.GetNewDeviceOwner(user, userRoles);
      //  ovnerID = DeviceTool.GetNewDeviceOwner(user, userRoles);
      //}

      var device = new Device
      {
        Id = null, Address = dev.Address, Currency = dev.Currency, Imei = dev.Imei,
        Phone = dev.Phone, TimeZone = dev.TimeZone, OwnerId = ovnerID
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
    [Authorize]
    public async Task<IActionResult> Edit(string id, [FromBody] DeviceAddModel dev)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      //var userRoles = await _userManager.GetRolesAsync(user);

      //var ownerId = dev.OwnerId;
      //if (string.IsNullOrEmpty(ownerId)) { ownerId = DeviceTool.GetNewDeviceOwner(user, userRoles); }
      var device = DeviceDbProvider.GetDeviceById(id);
      device.Imei = dev.Imei;
      device.Address = dev.Address;
      device.Currency = dev.Currency;
      device.Phone = dev.Phone;
      device.TimeZone = dev.TimeZone;

      //var device = new Device
      //{
      //  Id = id, Address = dev.Address, Currency = dev.Currency, Imei = dev.Imei,
      //  Phone = dev.Phone, TimeZone = dev.TimeZone//, OwnerId = ownerId
      //};

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
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var device = DeviceDbProvider.GetDeviceById(id);
      if (device == null) return NotFound("Автомат не найден!");
      
      //var userRoles = await _userManager.GetRolesAsync(user);
      if (user.Role == Roles.Technician)
      {
        if (user.OwnerId != device.OwnerId) return StatusCode(403, "Запрещено удалять чужие автоматы!"); 
      }

      //if (device.OwnerId == user.Id) DeviceDbProvider.RemoveDevice(device);
      //else
      //{
      //  var deviceOwner = await _userManager.FindByIdAsync(device.OwnerId);

      //  //todo check role
      //  var useRoles = await _userManager.GetRolesAsync(user);

      //  if (!useRoles.Contains(Roles.SuperAdmin)) return StatusCode(403, "Доступ запрещен!"); //Forbid("Доступ запрещен!");
      //}

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
    [Authorize]
    public async Task<IActionResult> GetSettings(string id) //Get(int size)
    {
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //if (user == null) return NotFound("Пользователь не найден!");
      //if (!user.Activated) return StatusCode(403, "Пользователь деактивирован!");

      var lastSettings = DeviceDbProvider.GetLastSettings(id, 0);
      if (lastSettings == null) return new ObjectResult(null);

      var settings = JsonConvert.DeserializeObject<MqttDeviceSettings>(lastSettings.Payload);
      return new ObjectResult(settings);
    }

    [HttpPut("/api/device/settings/{imei}")]
    [Authorize]
    public async Task<IActionResult> SetSettings(string imei, [FromBody] PayLoadValue payLoad) //Get(int size)
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
        return BadRequest(ex.Message);

        //Response.StatusCode = 400;
        //await Response.WriteAsync(ex.Message);
      }
    }

    [HttpGet("/api/device/info/{id}")]
    [Authorize]
    public async Task<IActionResult> GetInfo(string id)
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


        //elm = retList.Find(f => f.Name == "tds");
        //if (elm != null) sortedList.Add(elm);
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
        return BadRequest(ex.Message);
      }
    }

    [HttpPut("/api/device/setOwner/{id}")]
    [Authorize]
    public async Task<IActionResult> SetOwner(string id, [FromBody] string ownerId) //Get(int size)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var user = await _userManager.FindByIdAsync(ownerId);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        //var userRoles = await _userManager.GetRolesAsync(user);
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
        return BadRequest(ex.Message);

        //Response.StatusCode = 400;
        //await Response.WriteAsync(ex.Message);
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
