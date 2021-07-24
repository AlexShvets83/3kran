using CommonVending;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using ThirdVendingWebApi.Models;
using ThirdVendingWebApi.Models.Users;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager) { _userManager = userManager; }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
      try
      {
        //check admin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var users = new List<ApplicationUser>();
        switch (admin.Role)
        {
          case Roles.SuperAdmin:
          case Roles.Admin:
            users = _userManager.Users.OrderBy(s => s.Id).ToList();

            //remove SA
            var sAdm = users.FindAll(f => f.Role == Roles.SuperAdmin);
            foreach (var sa in sAdm) { users.Remove(sa); }

            break;

          case Roles.Dealer:
          case Roles.Owner:
            users = UserDbProvider.GetUserDescendants(admin.Id);
            break;
          case Roles.DealerAdmin:
            users = UserDbProvider.GetUserDescendants(admin.OwnerId);
            var dls = users.FindAll(f => f.Role == Roles.Dealer);
            foreach (var dl in dls) { users.Remove(dl); }

            break;
          case Roles.Technician:
            //users = new List<ApplicationUser>();
            if (!string.IsNullOrEmpty(admin.OwnerId))
            {
              users.Add(await _userManager.FindByIdAsync(admin.OwnerId));
            }

            break;
        }

        //var users = _userManager.Users.OrderBy(s => s.Id).ToList();
        var retUsers = users.Select(user => user.GetNewObj<UserAccount>()).ToList();

        //_userManager.GetUsersInRoleAsync(Roles.Technician);
        //todo check role

        //var superadmin = retUsers.FindAll(f => f.Role == Roles.SuperAdmin);
        //foreach (var sa in superadmin)
        //{
        //  retUsers.Remove(sa);
        //}

        //   </api/devices?page=0&size=1000>; rel="last",</api/devices?page=0&size=1000>; rel="first"
        //var headerStr = $@"</api/users?page=1&size={size}>; rel=""next"",</api/users?page=5&size={size}>; rel=""last"",</api/users?page=0&size={size}>; rel=""first""";
        //Response.Headers.Add("Link", headerStr);
        //Response.Headers.Add("X-Total-Count", retUsers.Count.ToString());

        //retUsers.Sort((x, y) => string.Compare(x.Id, y.Id, StringComparison.Ordinal));

        return new ObjectResult(retUsers);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("getUserById/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string id)
    {
      try
      {
        //check admin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var user = await _userManager.FindByIdAsync(id);
        var retUser = user.GetNewObj<UserAccount>();

        //todo check role
        return new ObjectResult(retUser);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    //[HttpGet("Authorities")]
    //[Authorize]
    //public async Task<IActionResult> Authorities()
    //{
    //  var user = await _userManager.GetUserAsync(HttpContext.User);
    //  if (user == null) return NotFound();
    //  if (!user.Activated.GetValueOrDefault()) return NotFound();

    //  var roles = await _userManager.GetRolesAsync(user);

    //  return new ObjectResult(roles.ToArray());
    //}

    [HttpGet("{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
      //check admin
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      ////check admin role
      //var adminRoles = await _userManager.GetRolesAsync(admin);
      //if (!adminRoles.Contains(Roles.Admin)) return StatusCode(403);

      var user = await _userManager.FindByNameAsync(email) ?? await _userManager.FindByEmailAsync(email);
      if (user == null) { return BadRequest("Invalid email!"); }

      var retUser = user.GetNewObj<UserAccount>();

      //var roles = await _userManager.GetRolesAsync(user);
      //retUser.Role = roles.Count > 0 ? roles[0] : null;
      return new ObjectResult(retUser);
    }

    /// <summary>
    /// </summary>
    /// <param name = "user"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] UserAccountAdminEdit user)
    {
      try
      {
        //check superadmin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        //check superadmin role
        //todo check role
        //var adminRoles = await _userManager.GetRolesAsync(admin);
        //if (!adminRoles.Contains(Roles.SuperAdmin)) return StatusCode(403);

        var userApp = await _userManager.FindByIdAsync(user.Id);
        if (userApp == null) return BadRequest("Invalid user account!");

        //userApp.Activated = user.Activated;
        userApp.FirstName = user.FirstName;
        userApp.LastName = user.LastName;
        userApp.Patronymic = user.Patronymic;

        //todo check email
        userApp.Email = user.Email;

        //todo check number
        userApp.PhoneNumber = user.PhoneNumber;

        userApp.Organization = user.Organization;
        userApp.City = user.City;
        userApp.LastModifiedBy = admin.UserName;
        userApp.LastModifiedDate = DateTime.Now;
        if (userApp.Role == Roles.Technician) { userApp.CommerceVisible = user.CommerceVisible; }

        if (userApp.Role == Roles.Owner) { userApp.OwnerId = user.OwnerId; }

        //if ((userApp.UserName != "admin@mail.com") && (admin.NormalizedEmail != user.Email.ToUpper()))
        if (userApp.UserName != "admin@mail.com")
        {
          //userApp.Activated = user.Activated;
          ////var userRoles = await _userManager.GetRolesAsync(userApp);
          //var roles = user.Authorities;
          //var addedRoles = roles.Except(userRoles);
          //var removedRoles = userRoles.Except(roles);
          //await _userManager.AddToRolesAsync(userApp, addedRoles);
          //await _userManager.RemoveFromRolesAsync(userApp, removedRoles);
        }

        //todo if super admin change password
        if (!string.IsNullOrEmpty(user.Password) && (user.Password.Length > 3) && (user.UserName != "admin@mail.com"))
        {
          var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
          var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

          var result = await passwordValidator.ValidateAsync(_userManager, userApp, user.Password);
          if (result.Succeeded) { userApp.PasswordHash = passwordHasher.HashPassword(userApp, user.Password); }
          else { return BadRequest("Пароль не соответствует шаблону!"); }
        }

        await _userManager.UpdateAsync(userApp);

        var usr = await _userManager.FindByIdAsync(user.Id);
        if (usr == null) return NotFound();

        var retUser = usr.GetNewObj<UserAccount>();

        await UserDbProvider.AddLog(new LogUsr
        {
          UserId = admin.Id,
          Email = admin.Email,
          Phone = admin.PhoneNumber,
          LogDate = DateTime.Now,
          Message = $"Пользователь {admin} обновил данные пользователя {retUser}"
        });

        //var retUserRoles = await _userManager.GetRolesAsync(usr);
        //retUser.Role = retUserRoles.Count > 0 ? retUserRoles[0] : null;
        return new ObjectResult(retUser);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    ///   Get allowed roles
    /// </summary>
    /// <returns></returns>
    [HttpGet("getRoles")]
    [Authorize]
    public async Task<IActionResult> GetRoles()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound();
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var returnRole = new HashSet<string>();

      foreach (var role in Roles.RolesPermissions) { returnRole.Add(role.Name); }

      //var roles = await _userManager.GetRolesAsync(user);

      //foreach (var role in roles)
      //{
      //  var r = Roles.RolesPermissions.Find(f => f.Name == role);

      //  var rr = Roles.RolesPermissions.FindAll(f => f.Code > r.Code);
      //  foreach (var rrs in rr) { returnRole.Add(rrs.Name); }
      //}

      //switch (roles)
      //{
      //  case Roles.Technician:
      //    break;
      //}

      return new ObjectResult(returnRole.ToArray());
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("getDealers/{countryId}")]
    public async Task<IActionResult> GetDealers(int countryId)
    {
      try
      {
        //var dealers = new List<ApplicationUser>();
        //var user = await _userManager.GetUserAsync(HttpContext.User);
        //if (user != null)
        //{
        //  if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        //  switch (user.Role)
        //  {
        //    case Roles.SuperAdmin:
        //    case Roles.Admin:
        //      dealers = _userManager.Users.Where(w => w.Role == Roles.Dealer).ToList();
        //      break;
        //    case Roles.Dealer:
        //      dealers = new List<ApplicationUser> {user};
        //      break;
        //    case Roles.DealerAdmin:
        //    case Roles.Technician:
        //      dealers = _userManager.Users.Where(w => w.Id == user.OwnerId).ToList();
        //      break;
        //  }
        //}
        //else
        //{
        //  dealers = _userManager.Users.Where(w => (w.CountryId == countryId) && (w.Role == Roles.Dealer)).ToList();
        //}

        //var dealerRole = await _roleManager.FindByNameAsync(Roles.Dealer);

        //var dealers = _userManager.Users.Where(w => w.CountryId == countryId).ToList();
        var dealers = _userManager.Users.Where(w => (w.CountryId == countryId) && (w.Role == Roles.Dealer)).ToList();

        //var dealers = countryId > 0
        //                ? _userManager.Users.Where(w => (w.CountryId == countryId) && (w.Role == Roles.Dealer)).ToList()
        //                : _userManager.Users.Where(w => w.Role == Roles.Dealer).ToList();

        var retDl = new List<DealerModel>();
        foreach (var dl in dealers)
        {
          var name = !string.IsNullOrEmpty(dl.Organization) ? dl.Organization : $"{dl.LastName} {dl.FirstName} {dl.Patronymic}";
          retDl.Add(new DealerModel {Id = dl.Id, Email = dl.Email, Name = string.IsNullOrEmpty(dl.AddDealerName) ? name : $"{name} {dl.AddDealerName}"});
        }

        return new ObjectResult(retDl);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("getDealers")]
    [Authorize]
    public async Task<IActionResult> GetDealersAuth()
    {
      try
      {
        var dealers = new List<ApplicationUser>();
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        switch (user.Role)
        {
          case Roles.SuperAdmin:
          case Roles.Admin:
            dealers = _userManager.Users.Where(w => w.Role == Roles.Dealer).ToList();
            break;
          case Roles.Dealer:
            dealers = new List<ApplicationUser> {user};
            break;
          case Roles.DealerAdmin:
          case Roles.Technician:
            dealers = _userManager.Users.Where(w => w.Id == user.OwnerId).ToList();
            break;
        }

        //var dealerRole = await _roleManager.FindByNameAsync(Roles.Dealer);

        //var dealers = _userManager.Users.Where(w => w.CountryId == countryId).ToList();
        //var dealers = (await _userManager.GetUsersInRoleAsync(dealerRole.Name)).ToList().FindAll(f => f.CountryId == countryId);

        //var dealers = countryId > 0
        //                ? _userManager.Users.Where(w => (w.CountryId == countryId) && (w.Role == Roles.Dealer)).ToList()
        //                : _userManager.Users.Where(w => w.Role == Roles.Dealer).ToList();

        var retDl = new List<DealerModel>();
        foreach (var dl in dealers)
        {
          var name = !string.IsNullOrEmpty(dl.Organization) ? dl.Organization : $"{dl.LastName} {dl.FirstName} {dl.Patronymic}";
          retDl.Add(new DealerModel {Id = dl.Id, Email = dl.Email, Name = string.IsNullOrEmpty(dl.AddDealerName) ? name : $"{name} {dl.AddDealerName}"});
        }

        return new ObjectResult(retDl);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("getOwners")]
    [Authorize]
    public async Task<IActionResult> GetOwners()
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        //var ownerRole = await _roleManager.FindByNameAsync(Roles.Owner);

        //todo get user role dealer
        //var dealers = _userManager.Users.Where(w => w.CountryId == countryId).ToList();
        //var owners = (await _userManager.GetUsersInRoleAsync(ownerRole.Name)).ToList().FindAll(f => f.Activated == true);
        var owners = new List<ApplicationUser>();
        switch (admin.Role)
        {
          case Roles.SuperAdmin:
          case Roles.Admin:
            owners = _userManager.Users.Where(w => (w.Role == Roles.Owner) && (w.Activated == true)).ToList();
            break;
          case Roles.Dealer:
            owners = UserDbProvider.GetUserDescendants(admin.Id, true);
            break;
          case Roles.DealerAdmin:
            owners = UserDbProvider.GetUserDescendants(admin.OwnerId, true);
            break;
        }

        var retUsers = owners.Select(user => user.GetNewObj<UserOwnerSetModel>()).ToList();

        return new ObjectResult(retUsers);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("getAllOwners")]
    [Authorize]
    public async Task<IActionResult> GetAllOwners()
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var owners = _userManager.Users.Where(w => (w.Role == Roles.Owner) && (w.Activated == true)).ToList();

        var retOwners = new List<DealerModel>();
        foreach (var own in owners)
        {
          var name = $"{own.LastName} {own.FirstName} {own.Patronymic} [{own.Email}]";
          retOwners.Add(new DealerModel {Id = own.Id, Email = own.Email, Name = name});
        }

        return new ObjectResult(retOwners);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("getAllDealers")]
    [Authorize]
    public async Task<IActionResult> GetAllDealers()
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var owners = _userManager.Users.Where(w => (w.Role == Roles.Dealer) && (w.Activated == true)).ToList();

        var retDealers = new List<DealerModel>();
        foreach (var dl in owners)
        {
          var name = !string.IsNullOrEmpty(dl.Organization) ? dl.Organization : $"{dl.LastName} {dl.FirstName} {dl.Patronymic}";
          retDealers.Add(new DealerModel {Id = dl.Id, Email = dl.Email, Name = string.IsNullOrEmpty(dl.AddDealerName) ? name : $"{name} {dl.AddDealerName}"});
        }

        //var retUsers = owners.Select(retUser => retUser.GetNewObj<UserOwnerSetModel>()).ToList();

        return new ObjectResult(retDealers);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("setActive/{id}")]
    [Authorize]
    public async Task<IActionResult> SetActive(string id, [FromBody] bool activated)
    {
      try
      {
        //check owner or admin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        var userApp = await _userManager.FindByIdAsync(id);
        if (userApp == null) return BadRequest("Invalid user account!");
        if (userApp.Role == Roles.SuperAdmin) return StatusCode(403, "Запрещено менять активность у супер-админа!");

        if (admin.Id == userApp.Id) return StatusCode(403, "Запрещено менять свою активность!");

        //var userRoles = await _userManager.GetRolesAsync(userApp);
        //if (userRoles.Contains(Roles.SuperAdmin))
        //  return StatusCode(403, "Запрещено менять активность у супер-админа!"); //return Forbid("Запрещено менять активность у супер-админа!");

        userApp.Activated = activated;

        ////todo if super admin change password
        //if (!string.IsNullOrEmpty(user.Password) && (user.Password.Length > 3) && (user.UserName != "admin@mail.com"))
        //{
        //  var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
        //  var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

        //  var result = await passwordValidator.ValidateAsync(_userManager, userApp, user.Password);
        //  if (result.Succeeded)
        //  {
        //    userApp.PasswordHash = passwordHasher.HashPassword(userApp, user.Password);
        //  }
        //}

        var result = await _userManager.UpdateAsync(userApp);
        if (result.Succeeded)
        {
          var wd = activated ? "активировал" : "деактивировал";
          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = admin.Id,
            Email = admin.Email,
            Phone = admin.PhoneNumber,
            LogDate = DateTime.Now,
            Message = $"Пользователь {admin} {wd} {userApp}"
          });

          return Ok();
        }

        var errors = new StringBuilder();
        foreach (var error in result.Errors) { errors.AppendLine($"Code - {error.Code} Description - {error.Description}"); }

        return BadRequest(errors);

        //var usr = await _userManager.FindByIdAsync(user.Id);
        //if (usr == null) return NotFound();

        //var retUser = usr.GetNewObj<UserAccount>();

        //var retUserRoles = await _userManager.GetRolesAsync(usr);

        //retUser.Authorities = retUserRoles.ToArray();

        //return new ObjectResult(retUser);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
      try
      {
        //check owner or admin
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if (string.IsNullOrEmpty(admin.Role) || (admin.Role == Roles.Technician)) return StatusCode(403, "Запрещено удалять!");

        var userApp = await _userManager.FindByIdAsync(id);
        if (userApp == null) return BadRequest("Invalid user account!");
        if (userApp.Role == Roles.SuperAdmin) return StatusCode(403, "Запрещено удалять!");

        if (admin.Id == userApp.Id) return StatusCode(403, "Запрещено удалять себя!");

        var descendants = UserDbProvider.GetUserDescendants(userApp.Id);
        
        if (descendants.Count > 1)
        {
          var strBld = new StringBuilder();
          foreach (var dc in descendants)
          {
            if (dc.Email == userApp.Email) continue;

            strBld.AppendLine($"{dc.Email};");
          }

          return StatusCode(403, $"У удаляемого пользователя есть сеть в кол-ве [{descendants.Count - 1}] пользователей!\r\n{strBld}");
        }

        //var userRoles = await _userManager.GetRolesAsync(userApp);
        //if (!userRoles.Contains(Roles.SuperAdmin))
        //  return StatusCode(403, "Запрещено удалять!"); //return Forbid("Запрещено менять активность у супер-админа!");

        //todo check children
        var result = await _userManager.DeleteAsync(userApp);
        if (result.Succeeded)
        {
          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = admin.Id,
            Email = admin.Email,
            Phone = admin.PhoneNumber,
            LogDate = DateTime.Now,
            Message = $"Пользователь {admin} удалил пользователя {userApp}"
          });
          return Ok();
        }

        var errors = new StringBuilder();
        foreach (var error in result.Errors) { errors.AppendLine($"Code - {error.Code} Description - {error.Description}"); }

        return BadRequest(errors);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }
  }
}
