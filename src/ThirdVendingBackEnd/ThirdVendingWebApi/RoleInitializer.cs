using System;
using System.Threading.Tasks;
using DeviceDbModel;
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

      if (await roleManager.FindByNameAsync(Roles.SuperAdmin) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin)); }
      if (await roleManager.FindByNameAsync(Roles.Admin) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.Admin)); }
      if (await roleManager.FindByNameAsync(Roles.Dealer) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.Dealer)); }
      if (await roleManager.FindByNameAsync(Roles.DealerAdmin) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.DealerAdmin)); }
      if (await roleManager.FindByNameAsync(Roles.Owner) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.Owner)); }
      if (await roleManager.FindByNameAsync(Roles.Technician) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.Technician)); }

      //if (await roleManager.FindByNameAsync(Roles.User) == null) { await roleManager.CreateAsync(new IdentityRole(Roles.User)); }
      
      if (await userManager.FindByNameAsync("BOSS") == null)
      {
        var boss = new ApplicationUser
        {
          Email = "boss@3kran.com", UserName = "BOSS", FirstName = "BOSS", LastName = "BOSS",
          Patronymic = "BOSS", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "ru", UserAlerts = 15, Organization = "3kran", City = "г Пермь",
          PhoneNumber = "79922313003", CountryId = 1, Role = Roles.SuperAdmin
        };
        try
        {
          var result = await userManager.CreateAsync(boss, password);
          if (result.Succeeded) await userManager.AddToRoleAsync(boss, Roles.SuperAdmin);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }

      if (await userManager.FindByNameAsync("Developer") == null)
      {
        var boss = new ApplicationUser
        {
          Email = "alex.a.shvets@gmail.com", UserName = "Developer", FirstName = "Developer", LastName = "Developer",
          Patronymic = "Developer", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "ru", UserAlerts = 15, Organization = "Developer", City = "Kyiv",
          PhoneNumber = "+380672333818", CountryId = 1, Role = Roles.SuperAdmin
        };
        try
        {
          var result = await userManager.CreateAsync(boss, password);
          if (result.Succeeded) await userManager.AddToRoleAsync(boss, Roles.SuperAdmin);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }
      
      if (await userManager.FindByNameAsync("admin3kran") == null)
      {
        //var admin = new ApplicationUser {Email = adminEmail, UserName = adminEmail};
        var admin = new ApplicationUser
        {
          Email = adminEmail, UserName = "admin3kran", FirstName = "Admin", LastName = "Admin",
          Patronymic = "Admin", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "en", UserAlerts = 15, Organization = "3kran", City = "г Пермь",
          PhoneNumber = "79922313003", CountryId = 1, Role = Roles.SuperAdmin
        };
        try
        {
          var result = await userManager.CreateAsync(admin, password);
          if (result.Succeeded) await userManager.AddToRoleAsync(admin, Roles.SuperAdmin);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
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
          PhoneNumber = "79922313003", CountryId = 1, Role = Roles.Technician
        };
        try
        {
          var result = await userManager.CreateAsync(user, password);
          if (result.Succeeded) await userManager.AddToRoleAsync(user, Roles.Technician);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }

      const string dealerEmail = "dealer@demo.3kran";
      if (await userManager.FindByNameAsync(dealerEmail) == null)
      {
        var dealer = new ApplicationUser
        {
          Email = dealerEmail, UserName = dealerEmail, FirstName = "Deaer", LastName = "Demo",
          Patronymic = "Demo", Activated = true, CreatedBy = "Default", CreatedDate = DateTime.Now,
          LangKey = "en", UserAlerts = 15, Organization = "3kran", City = "г Пермь",
          PhoneNumber = "79922313003", CountryId = 1, Role = Roles.Dealer
        };
        try
        {
          var result = await userManager.CreateAsync(dealer, password);
          if (result.Succeeded) { await userManager.AddToRoleAsync(dealer, Roles.Dealer); }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
      }
    }
  }
}
