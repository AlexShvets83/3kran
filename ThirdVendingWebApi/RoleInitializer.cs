﻿using System;
using System.Threading.Tasks;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ThirdVendingWebApi
{
  public static class RoleInitializer
  {
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      var adminEmail = "admin@mail.com";
      var password = "!Qw12345";

      if (await roleManager.FindByNameAsync(Roles.Admin) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.Admin)); }

      if (await roleManager.FindByNameAsync(Roles.User) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.User)); }

      if (await userManager.FindByNameAsync(adminEmail) == null)
      {
        //var admin = new ApplicationUser {Email = adminEmail, UserName = adminEmail};
        var admin = new ApplicationUser
        {
          Email = adminEmail, UserName = adminEmail, FirstName = "Admin", LastName = "Admin",
          Patronymic = "Admin", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "en", UserAlerts = 15, Organization = "3kran", City = "г Пермь",
          PhoneNumber = "+79922313003"
        };
        var result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(admin, Roles.User);
          await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
      }

      const string userEmail = "demo@demo.3kran";
      if (await userManager.FindByNameAsync(userEmail) == null)
      {
        var user = new ApplicationUser
        {
          Email = userEmail, UserName = userEmail, FirstName = "Demo", LastName = "Demo",
          Patronymic = "Demo", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "en", UserAlerts = 15, Organization = "3kran", City = "г Пермь",
          PhoneNumber = "+79922313003"
        };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded) { await userManager.AddToRoleAsync(user, Roles.User); }
      }
    }
  }
}
