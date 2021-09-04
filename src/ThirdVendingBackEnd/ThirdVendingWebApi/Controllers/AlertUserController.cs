using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AlertUserController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public AlertUserController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        return new ObjectResult(user);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }
  }
}
