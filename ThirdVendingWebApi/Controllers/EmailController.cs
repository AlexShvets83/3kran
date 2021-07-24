using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    
    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "userManager"></param>
    /// <param name = "signInManager"></param>
    /// <param name = "emailSender"></param>
    public EmailController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
    }

    [HttpPost("sentMail")]
    [Authorize]
    public async Task<IActionResult> SentMail([FromBody] EmailSendModel model)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        
        try
        {
          //var addressees = string.Empty;
          var addressees = new StringBuilder();
          for (int i = 0; i < model.Addressees.Count; i++)
          {
            addressees.Append(model.Addressees[i]);
            if (i != model.Addressees.Count - 1) addressees.Append(",");
          }

          //foreach (var email in model.Addressees)
          //{
          //  addressees += $"{email},";
          //}

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
        catch (Exception ex)
        {
          //InviteDbProvider.RemoveInvite(id);
          return BadRequest(ex.Message);
        }
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
