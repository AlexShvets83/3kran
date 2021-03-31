using System;
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

      if (await roleManager.FindByNameAsync("ROLE_ADMIN") == null) { await roleManager.CreateAsync(new IdentityRole("ROLE_ADMIN")); }

      if (await roleManager.FindByNameAsync("ROLE_USER") == null) { await roleManager.CreateAsync(new IdentityRole("ROLE_USER")); }

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
          await userManager.AddToRoleAsync(admin, "ROLE_USER");
          await userManager.AddToRoleAsync(admin, "ROLE_ADMIN");
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
        if (result.Succeeded) { await userManager.AddToRoleAsync(user, "ROLE_USER"); }
      }
    }
  }
}
