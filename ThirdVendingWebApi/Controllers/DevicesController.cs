using System;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel;
using ThirdVendingWebApi.Models;
using ThirdVendingWebApi.Tools;

namespace ThirdVendingWebApi.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class DevicesController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    public DevicesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> Get()
    {
      var devList = DeviceDbProvider.GetDevices();
      //todo add last state
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //if (user == null) return NotFound();
      //if (!user.Activated) return NotFound();

      //var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
      //Response.Headers.Add("Link", headerStr);
      //Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(devList);
    }

    [HttpGet("/api/_search/devices")]
    [Authorize]
    public async Task<IActionResult> GetDevices(string query, int page, int size, string sort) //Get(int size)
    {
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //if (user == null) return NotFound();
      //if (!user.Activated) return NotFound();
      //http: //95.183.10.198/api/_search/devices?query=f2f196db-4cb1-47a4-9b74-2f602c2c5517&page=0&size=10&sort=id.keyword,desc
      var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
      Response.Headers.Add("Link", headerStr);
      Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(new string [0]);
    }

    [HttpPost("/api/device")]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] DeviceAddModel dev)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated) return NotFound("Пользователь деактивирован!");
      var userRoles = await _userManager.GetRolesAsync(user);

      var ovnerID = dev.OwnerId;
      if (string.IsNullOrEmpty(ovnerID))
      {
        //ovnerID = userRoles.Contains(Roles.Owner) ? user.Id : DeviceTool.GetNewDeviceOwner(user, userRoles);
        ovnerID = DeviceTool.GetNewDeviceOwner(user, userRoles);
      }

      var device = new Device
      {
        Id = Guid.NewGuid().ToString(),
        Address = dev.Address,
        Currency = dev.Currency,
        Imei = dev.DeviceId,
        Phone = dev.Phone,
        TimeZone = dev.TimeZone,
        OwnerId = ovnerID
      };

      if (await DeviceDbProvider.AddDevice(device)) return Ok();

      return BadRequest();
    }






    /// <summary>
    /// Удалить автомат
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("/api/device/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return Forbid("Доступ запрещен!");
      if (!user.Activated) return Forbid("Доступ запрещен!");

      var device = DeviceDbProvider.GetDeviceById(id);
      if (device == null) return NotFound("Автомат не найден!");

      if (device.OwnerId == user.Id) DeviceDbProvider.RemoveDevice(device);
      else
      {
        var deviceOwner = await _userManager.FindByIdAsync(device.Id);
        //todo check role
        var useRoles = await _userManager.GetRolesAsync(user);
        
        if (!useRoles.Contains(Roles.SuperAdmin)) return Forbid("Доступ запрещен!");
      }
      
      return Ok();
    }
  }
}
