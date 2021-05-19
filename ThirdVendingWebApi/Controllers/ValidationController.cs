using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValidationController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public ValidationController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet("/api/validation/email/{email}")]
    public async Task<IActionResult> Email(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) return Ok();

      return BadRequest();
    }

    [HttpGet("/api/validation/imei/{imei}")]
    public IActionResult Imei(string imei)
    {
      var user = DeviceDbProvider.GetDeviceByImei(imei);
      if (user == null) return Ok();

      return BadRequest();
    }

    //[HttpGet("/api/validation/phone/{phone}")]
    [HttpGet("/api/validation")]
    public IActionResult Phone(string phone)
    {
      if (string.IsNullOrEmpty(phone)) return BadRequest();

      //var isPlus = phone.StartsWith("+");
      var np = new string(phone.Where(char.IsDigit).ToArray());

      //if (isPlus) np = np.Insert(0, "+");

      var user = _userManager.Users.Where(w => w.PhoneNumber == np);
      if (!user.Any()) return Ok();

      return BadRequest();
    }
  }
}
