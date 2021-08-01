using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AppSettingsController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public AppSettingsController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
      if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено изменять файлы!");

      var settings = AppSettingsDbProvider.GetAppSettings();
      ApplicationSettings.Set(settings);
      return Ok(settings);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(AppSettings data)
    {
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
      if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено изменять файлы!");

      await AppSettingsDbProvider.SetAppSettings(data);
      ApplicationSettings.Set(data);
      return Ok();
    }
  }
}
