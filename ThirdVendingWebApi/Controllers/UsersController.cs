using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           RoleManager<IdentityRole> roleManager,
                           IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(string query, int page, int size, string sort)
    {
      try
      {
        var users = _userManager.Users.ToList();
        var retUsers = new List<UserAccount>();
        foreach (var user in users)
        {
          var retUser = user.GetNewObj<UserAccount>();
          var roles = await _userManager.GetRolesAsync(user);
          retUser.Authorities = roles.ToArray();
          retUsers.Add(retUser);
        } 
      
        //   </api/devices?page=0&size=1000>; rel="last",</api/devices?page=0&size=1000>; rel="first"
        var headerStr = $@"</api/users?page=1&size={size}>; rel=""next"",</api/users?page=5&size={size}>; rel=""last"",</api/users?page=0&size={size}>; rel=""first""";
        Response.Headers.Add("Link", headerStr);
        Response.Headers.Add("X-Total-Count", users.Count.ToString());
        return new ObjectResult(retUsers);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("Authorities")]
    [Authorize]
    public async Task<IActionResult> Authorities()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound();
      if (!user.Activated) return NotFound();

      var roles = await _userManager.GetRolesAsync(user);

      return new ObjectResult(roles.ToArray());
    }

    [HttpGet("{email}")]
    //[Authorize(Roles = "ROLE_ADMIN")]
    public async Task<IActionResult> Get(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);
      if (user == null) { return BadRequest("Invalid email!"); }
      
      var retUser = user.GetNewObj<UserAccount>();
      var roles = await _userManager.GetRolesAsync(user);
      retUser.Authorities = roles.ToArray();
      return new ObjectResult(retUser);
    }

    [HttpPut()]
    public async Task<IActionResult> Put([FromBody] UserAccountAdminEdit user)
    {
      try
      {
        var userApp = await _userManager.FindByIdAsync(user.Id);
        if (userApp == null) return BadRequest("Invalid user account!");

        userApp.CopyObjectProperties(user);

        //without roles

        userApp.InfoEmails = string.Empty;
        if ((user.InfoEmails != null) || (user.InfoEmails.Length > 0))
        {
          for (var i = 0; i < user.InfoEmails.Length; i++)
          {
            userApp.InfoEmails += user.InfoEmails[i];
            if (i != user.InfoEmails.Length - 1) userApp.InfoEmails += ",";
          }
        }

        var nesAlerts = 0;
        for (var i = 0; i < user.Alerts.Length; i++)
        {
          if (user.Alerts[i].Active) nesAlerts |= 1 << i;
        }

        userApp.UserAlerts = nesAlerts;
        await _userManager.UpdateAsync(userApp);
      }
      catch (Exception ex)
      {

      }
    }
  }
}
