using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "userManager"></param>
    /// <param name = "emailSender"></param>
    public EmailController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    {
      _userManager = userManager;
      _emailSender = emailSender;
    }

    [HttpPost("sendMail")]
    [Authorize]
    public async Task<IActionResult> SendMail([FromBody] EmailSendModel model)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var addressees = new StringBuilder();
        for (var i = 0; i < model.Addressees.Count; i++)
        {
          addressees.Append(model.Addressees[i]);
          if (i != model.Addressees.Count - 1) addressees.Append(',');
        }

        await _emailSender.SendEmailAsync(addressees.ToString(), model.EmailTheme, model.EmailBody);

        //await UserDbProvider.AddLog(new LogUsr
        //{
        //  UserId = user.Id,
        //  Email = user.Email,
        //  Phone = user.PhoneNumber,
        //  LogDate = DateTime.Now,
        //  Message = $"Пользователь {user} отослал приглашение [{invite.Email}] на роль [{invite.Role}]"
        //});
        return Ok();
      }
      catch (Exception ex) { return StatusCode(500, $"Internal server error: {ex}"); }
    }
  }
}
