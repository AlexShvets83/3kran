﻿using CommonVending;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceDbModel;
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
        var users = _userManager.Users.OrderBy(s => s.Id).ToList();
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
        Response.Headers.Add("X-Total-Count", retUsers.Count.ToString());

        //retUsers.Sort((x, y) => string.Compare(x.Id, y.Id, StringComparison.Ordinal));

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
    public async Task<IActionResult> Get(string email)
    {
      //check admin
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Invalid ADMIN account!");
      if (!admin.Activated) return BadRequest("Invalid ADMIN activation!");
      
      //check admin role
      var adminRoles = await _userManager.GetRolesAsync(admin);
      if (!adminRoles.Contains(Roles.Admin)) return Forbid();

      var user = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);
      if (user == null) { return BadRequest("Invalid email!"); }
      
      var retUser = user.GetNewObj<UserAccount>();
      var roles = await _userManager.GetRolesAsync(user);
      retUser.Authorities = roles.ToArray();
      return new ObjectResult(retUser);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UserAccountAdminEdit user)
    {
      try
      {
        //check admin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Invalid ADMIN account!");
        if (!admin.Activated) return BadRequest("Invalid ADMIN activation!");
      
        //check admin role
        var adminRoles = await _userManager.GetRolesAsync(admin);
        if (!adminRoles.Contains(Roles.Admin)) return Forbid();
        
        var userApp = await _userManager.FindByIdAsync(user.Id);
        if (userApp == null) return BadRequest("Invalid user account!");

        //userApp.Activated = user.Activated;
        userApp.FirstName = user.FirstName;
        userApp.LastName = user.LastName;
        userApp.Patronymic = user.Patronymic;
        userApp.Email = user.Email;
        userApp.PhoneNumber = user.PhoneNumber;
        userApp.Organization = user.Organization;
        userApp.City = user.City;
        userApp.LastModifiedBy = admin.UserName;
        userApp.LastModifiedDate = DateTime.Now;

        if ((userApp.UserName != "admin@mail.com") && (admin.NormalizedEmail != user.Email.ToUpper()))
        {
          userApp.Activated = user.Activated;
          var userRoles = await _userManager.GetRolesAsync(userApp);
          var roles = user.Authorities;
          var addedRoles = roles.Except(userRoles);
          var removedRoles = userRoles.Except(roles);
          await _userManager.AddToRolesAsync(userApp, addedRoles);
          await _userManager.RemoveFromRolesAsync(userApp, removedRoles);
        }

        if (!string.IsNullOrEmpty(user.Password) && (user.Password.Length > 3) && (user.UserName != "admin@mail.com"))
        {
          var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
          var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

          var result = await passwordValidator.ValidateAsync(_userManager, userApp, user.Password);
          if (result.Succeeded)
          {
            userApp.PasswordHash = passwordHasher.HashPassword(userApp, user.Password);
          }
        }

        await _userManager.UpdateAsync(userApp);

        var usr = await _userManager.FindByIdAsync(user.Id);
        if (usr == null) return NotFound();
        
        var retUser = usr.GetNewObj<UserAccount>();

        var retUserRoles = await _userManager.GetRolesAsync(usr);

        retUser.Authorities = retUserRoles.ToArray();
        
        return new ObjectResult(retUser);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}
