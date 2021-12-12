using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ThirdVendingWebApi.Tools;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LogController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public LogController(UserManager<ApplicationUser> userManager) { _userManager = userManager; }

    [HttpGet("/api/log!1231easdda!11!2@")]
    public IActionResult Getlog()
    {
      var info = DeviceTool.WriteCommand("journalctl -u 3kran-web.service -r");
      return new ObjectResult(info);
    }

    //[Authorize]
    //public async Task<IActionResult> Getlog()
    //{
    //  var admin = await _userManager.GetUserAsync(HttpContext.User);
    //  if (admin == null) return NotFound("Пользователь не найден!");
    //  if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
    //  if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено просматривать журнал!");

    //  var info = DeviceTool.WriteCommand("journalctl -u 3kran-web.service");
    //  return new ObjectResult(info);
    //}

    [HttpGet("getLog")]
    [Authorize]
    public async Task<IActionResult> GetUsersLog()
    {
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
      if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено просматривать журнал!");

      return new ObjectResult(UserDbProvider.GetUserLog());
    }
  }
}
