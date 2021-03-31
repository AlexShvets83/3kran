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
      var headerStr = $@"</api/users?page=1&size={size}>; rel=""next"",</api/users?page=35&size={size}>; rel=""last"",</api/users?page=0&size={size}>; rel=""first""";
      Response.Headers.Add("Link:", "20");
      return new ObjectResult(retUsers);
    }
  }
}
