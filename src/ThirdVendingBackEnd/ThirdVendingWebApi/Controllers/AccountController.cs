using CommonVending;
using CommonVending.Crypt;
using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;
using ThirdVendingWebApi.Models.Users;

namespace ThirdVendingWebApi.Controllers
{
  /// <summary>
  ///   Account controller
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;

    //private readonly IConfiguration _configuration;
    //private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "userManager"></param>
    /// <param name = "signInManager"></param>
    /// <param name = "emailSender"></param>
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)

      //RoleManager<IdentityRole> roleManager,
      //IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;

      //_roleManager = roleManager;
      //_configuration = configuration;
    }

    /// <summary>
    ///   Get JWT token
    /// </summary>
    /// <param name = "person"></param>
    /// <returns>JWT token</returns>
    /// <response code = "400">Bad Request - If invalid username or password.</response>
    [HttpPost("/api/Authenticate")]
    [AllowAnonymous]
    public async Task Authenticate([FromBody] Person person)
    {
      try
      {
        var appUser = //_userManager.Users.SingleOrDefault(r => r.UserName == person.UserName) ??
          _userManager.Users.SingleOrDefault(r => r.NormalizedEmail == person.UserName.ToUpper()) ?? _userManager.Users.SingleOrDefault(r => r.PhoneNumber == person.UserName);

        if ((appUser != null) && appUser.Activated.GetValueOrDefault())
        {
          var result = await _signInManager.CheckPasswordSignInAsync(appUser, person.Password, false);
          if (result.Succeeded)
          {
            var roles = await _userManager.GetRolesAsync(appUser);
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
              new(JwtRegisteredClaimNames.Email, appUser.Email), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), new(ClaimTypes.NameIdentifier, appUser.Id)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // создаем JWT-токен
            var jwt = new JwtSecurityToken(AuthOptions.JwtIssuer, AuthOptions.JwtAudience, notBefore: now, claims: claims,
                                           expires: now.Add(TimeSpan.FromMinutes(AuthOptions.JwtExpireMinutes)),
                                           signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //var response = new {id_token = encodedJwt, username = appUser.UserName};
            var response = new {id_token = encodedJwt};

            // сериализация ответа
            Response.ContentType = "application/json";
            Response.Headers.Add("Authorization", "Bearer " + encodedJwt);
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings {Formatting = Formatting.Indented}));
          }
          else
          {
            Response.StatusCode = 400;
            await Response.WriteAsync("Invalid username or password.");
          }
        }
        else
        {
          Response.StatusCode = 400;
          await Response.WriteAsync("Invalid username or password.");
        }
      }
      catch (Exception ex)
      {
        Response.StatusCode = 400;
        await Response.WriteAsync(ex.Message);
      }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    /// <response code = "401">Unauthorized - If bad JSON Web Token (JWT)</response>
    /// <response code = "403">Forbidden - If your role hasn't permission</response>
    [HttpGet]
    [Authorize]
    [Produces(typeof(UserAccount))]
    public async Task<IActionResult> Get()
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound();
      if (!user.Activated.GetValueOrDefault()) return NotFound();

      var retUser = user.GetNewObj<UserAccount>();

      //var roles = await _userManager.GetRolesAsync(user);

      //var usrRoles = roles.ToArray();
      //retUser.Role = roles.Count > 0 ? roles[0] : null;
      //for (int i = 0; i < retUser.Alerts.Count; i++)
      //{
      //  retUser.Alerts[i].Active = ((retUser.UserAlerts >> i) & 1) == 1;
      //}

      return new ObjectResult(retUser);
    }

    //// GET api/<ValuesController>/5
    //[HttpGet("{id}")]
    //public string Get(int id)
    //{
    //  return "value";
    //}

    /// <summary>
    ///   Обновление изменение профиля пользователя
    /// </summary>
    /// <param name = "user"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task Post([FromBody] UserAccountAdd user)
    {
      var userApp = await _userManager.GetUserAsync(HttpContext.User);
      if ((userApp == null) || (userApp.UserName != user.UserName)) return;

      //userApp.CopyObjectProperties(user);

      //without roles
      userApp.LastModifiedBy = userApp.UserName;
      userApp.LastModifiedDate = DateTime.Now;

      userApp.InfoEmails = string.Empty;
      if ((user.InfoEmails != null) || (user.InfoEmails?.Length > 0))
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
      await UserDbProvider.AddLog(new LogUsr
      {
        UserId = userApp.Id, Email = userApp.Email, Phone = userApp.PhoneNumber, LogDate = DateTime.Now,
        Message = $"Пользователь {userApp} обновил свои данные"
      });
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePsw pws)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      if (!string.IsNullOrEmpty(pws.NewPassword) && (pws.NewPassword.Length > 3))
      {
        var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
        var result = await passwordValidator.ValidateAsync(_userManager, user, pws.NewPassword);
        if (result.Succeeded)
        {
          await _userManager.ChangePasswordAsync(user, pws.CurrentPassword, pws.NewPassword);

          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
            Message = $"Пользователь {user} изменил свой пароль"
          });
          return Ok();
        }

        return BadRequest("Пароль не соответствует шаблону!");
      }

      return BadRequest("Пароль не соответствует шаблону!");
    }

    [HttpPut("change-credentials")]
    [Authorize]
    public async Task<IActionResult> ChangeCredentials([FromBody] UserCredentials userEdit)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      user.Email = userEdit.Email;
      user.PhoneNumber = userEdit.PhoneNumber;

      var res = await _userManager.UpdateAsync(user);
      if (!res.Succeeded) return BadRequest("Не удалось сменить email или телефон!");

      if (!string.IsNullOrEmpty(userEdit.NewPassword) && (userEdit.NewPassword.Length > 3))
      {
        var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
        var result = await passwordValidator.ValidateAsync(_userManager, user, userEdit.NewPassword);
        if (result.Succeeded)
        {
          var pRes = await _userManager.ChangePasswordAsync(user, userEdit.CurrentPassword, userEdit.NewPassword);
          if (pRes.Succeeded)
          {
            await UserDbProvider.AddLog(new LogUsr
            {
              UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
              Message = $"Пользователь {user} изменил свой пароль"
            });
            return Ok();
          }

          return BadRequest("Пароль не был изменен!");
        }

        return BadRequest("Пароль не соответствует шаблону!");
      }

      await UserDbProvider.AddLog(new LogUsr
      {
        UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
        Message = $"Пользователь {user} изменил свои учетные данные"
      });
      return Ok();
    }

    /// <summary>
    ///   Регистрация нового пользователя
    /// </summary>
    /// <param name = "user"></param>
    /// <returns></returns>
    [HttpPost("/api/Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserAccountRegister user)
    {
      try
      {
        //var resp = new
        //{
        //  entityName = "userManagement", errorKey = "userexists", type = "https://www.jhipster.tech/problem/login-already-used", title = "Login name already used!",
        //  status = 400, message = "error.userexists", "params" = "userManagement"
        //};
        //var retBad =
        //  @"{""entityName"":""userManagement"",""errorKey"":""userexists"",""type"":""https://www.jhipster.tech/problem/login-already-used"",""title"":""Login name already used!"",""status"":400,""message"":""error.userexists"",""params"":""userManagement""}";

        var userCheck = await _userManager.FindByEmailAsync(user.Email);
        if (userCheck != null) { return BadRequest("Пользователь с таким email уже зарегистрирован!"); }

        userCheck = await _userManager.FindByNameAsync(user.UserName);
        if (userCheck != null) { return BadRequest("Пользователь с таким логином уже зарегистрирован!"); }

        userCheck = await _userManager.FindByNameAsync(user.PhoneNumber);
        if (userCheck != null) { return BadRequest("Пользователь с таким телефоном уже зарегистрирован!"); }

        ApplicationUser owner = null;

        if (!string.IsNullOrEmpty(user.DealerEmail))
        {
          owner = await _userManager.FindByEmailAsync(user.DealerEmail);
          if (owner == null) { return BadRequest("Дилер с таким email не существует!"); }
        }

        var ownerId = owner?.Id;
        var role = Roles.Owner;
        InviteRegistration invite = null;
        if (!string.IsNullOrEmpty(user.InviteCode))
        {
          //get invite
          invite = InviteDbProvider.GetInvite(user.InviteCode);
          if (invite == null) return NotFound("Код приглашения недействителен!");

          role = invite.Role;
          if (!string.IsNullOrEmpty(invite.OwnerId))
          {
            owner = await _userManager.FindByIdAsync(invite.OwnerId);
            if (owner == null) { return BadRequest("Отправитель с таким email не существует!"); }

            user.CountryId = owner.CountryId;

            if ((role == Roles.Admin) || (role == Roles.Dealer)) ownerId = null;
            else ownerId = owner.Id;
          }
        }

        var userApp = new ApplicationUser();
        userApp.CopyObjectProperties(user);

        var creator = user.UserName;
        if (!string.IsNullOrEmpty(user.InviteCode) && (owner != null)) creator = owner.Email;

        userApp.Activated = string.IsNullOrEmpty(user.InviteCode) ? null : true;
        userApp.CreatedBy = creator;
        userApp.CreatedDate = DateTime.Now;
        userApp.UserAlerts = 15;

        userApp.CountryId = owner?.CountryId ?? user.CountryId;
        userApp.OwnerId = ownerId;
        userApp.Role = role;

        var result = await _userManager.CreateAsync(userApp, user.Password);
        if (result.Succeeded)
        {
          //delete invite
          if (invite != null) InviteDbProvider.RemoveInvite(invite.Id);
          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = userApp.Id, Email = userApp.Email, Phone = userApp.PhoneNumber, LogDate = DateTime.Now,
            Message = $"Пользователь {userApp} зарегистрирован"
          });
          return Ok();
        }

        return BadRequest(result.Errors);
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpPost("reset-password/init")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] string email)
    {
      try
      {
        //string email;
        //using (var bodyStream = new StreamReader(Request.Body)) { email = await bodyStream.ReadToEndAsync(); }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
          //var resp = new {type = "https://www.jhipster.tech/problem/email-not-found", title = "Email address not registered", status = 400};
          return BadRequest();
        }

        var model = new ChangePasswordInitModel {Id = user.Id, Email = user.Email, ExpiryDateTime = DateTime.Now.AddDays(1)};
        var strModel = JsonConvert.SerializeObject(model);

        //var code = Crypto.Encrypt(user.Id);
        var code = Crypto.Encrypt(strModel);
        var ecode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var host = HttpContext.Request.Host;
        var sch = HttpContext.Request.Scheme;

        //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        //var callbackUrl = Url.Page(
        //                           "/reset/finish",
        //                           pageHandler: null,
        //                           values: new {key = code},
        //                           protocol: Request.Scheme);

        //var message = $"Уважаемый {user.FirstName} {user.LastName}\r\nДля вашего аккаунта в системе мониторинга «Третий кран» был запрошен сброс пароля.Чтобы сбросить пароль нажмите на";
        var message = new StringBuilder();
        message.AppendLine($"<div>Уважаемый {user.FirstName} {user.Patronymic}!</div><br />");
        message.AppendLine("<div>Для вашего аккаунта в системе мониторинга «Третий кран» был запрошен сброс пароля.</div><br />");
        message.AppendLine("<div>Чтобы сбросить пароль нажмите на");

        //var callbackUrl = $"{sch}://{host}/#/reset/finish?key={ecode}";
        var callbackUrl = $"{sch}://{host}/Account/ResetPsw?key={ecode}";
        await _emailSender.SendEmailAsync(email, "Запрос на сброс пароля", $"{message} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылку</a>.</div>");

        await UserDbProvider.AddLog(new LogUsr
        {
          UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
          Message = $"Пользователь {user} подал заявку на сброс пароля"
        });

        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return BadRequest(ex.Message);
      }
    }

    ///api/account/reset-password/finish
    [HttpPost("reset-password/finish")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPasswordFinish([FromBody] ChangePasswordModel model)
    {
      var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Key));
      var strPswModel = Crypto.Decrypt(code);

      var pswModel = JsonConvert.DeserializeObject<ChangePasswordInitModel>(strPswModel);
      if (pswModel.ExpiryDateTime < DateTime.Now) return BadRequest("Время жизни ссылки истекло!");

      var user = await _userManager.FindByIdAsync(pswModel.Id);
      if ((user == null) || (user.NormalizedEmail != pswModel.Email.ToUpper())) { return BadRequest("Пользователь не найден!"); }

      var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
      var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

      var result = await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
      if (result.Succeeded)
      {
        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
        await _userManager.UpdateAsync(user);

        await UserDbProvider.AddLog(new LogUsr
        {
          UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
          Message = $"Пользователь {user} изменил свой пароль через сброс"
        });

        return Ok();
      }

      var errors = new StringBuilder();
      foreach (var error in result.Errors)
      {
        //ModelState.AddModelError(string.Empty, error.Description);
        errors.AppendLine(error.Description);
      }

      return BadRequest(errors);
    }

    /// <summary>
    ///   Обновление изменение профиля пользователя
    /// </summary>
    /// <param name = "userEdit"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] UserAccountEdit userEdit)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      user.FirstName = userEdit.FirstName;
      user.LastName = userEdit.LastName;
      user.Patronymic = userEdit.Patronymic;
      user.Organization = userEdit.Organization;
      user.City = userEdit.City;

      var result = await _userManager.UpdateAsync(user);
      if (result.Succeeded)
      {
        await UserDbProvider.AddLog(new LogUsr
        {
          UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
          Message = $"Пользователь {user} изменил свои данные"
        });
        return Ok();
      }

      return BadRequest("Профиль не был изменен!");
    }

    ///// <summary>
    ///// Удалить автомат
    ///// </summary>
    ///// <param name="id"></param>
    ///// <returns></returns>
    //[HttpDelete("{id}")]
    //[Authorize(Roles = Roles.SuperAdmin)]
    //public async Task<IActionResult> Delete(int id)
    //{
    //  //check admin
    //  var admin = await _userManager.GetUserAsync(HttpContext.User);
    //  if (admin == null) return NotFound("Invalid ADMIN account!");
    //  if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Invalid ADMIN activation!");

    //  //check admin role
    //  var adminRoles = await _userManager.GetRolesAsync(admin);
    //  if (!adminRoles.Contains(Roles.SuperAdmin)) return StatusCode(403);

    //  return Ok();
    //}

    /// <summary>
    ///   Получить список стран
    /// </summary>
    /// <returns></returns>
    [HttpGet("/api/countries")]
    public IActionResult GetCountries()
    {
      var countries = DeviceDbProvider.GetCountries();
      var returnList = new List<CountryModel>();
      foreach (var country in countries) { returnList.Add(new CountryModel {Id = country.Id, Name = country.Name}); }

      //var countries = await new TaskFactory().StartNew(DeviceDbProvider.GetCountries);
      return new ObjectResult(returnList);
    }

    [HttpGet("invite/{id}")]
    public async Task<IActionResult> GetInviteData(string id)
    {
      var invite = InviteDbProvider.GetInvite(id);
      if (invite == null) return NotFound("Код приглашения недействителен!");

      var ownerId = invite.OwnerId;
      var countryId = 1;
      string ownerEmail = null;
      if (!string.IsNullOrEmpty(ownerId))
      {
        var userApp = await _userManager.FindByIdAsync(ownerId);
        if (userApp == null) return BadRequest("Invalid user account!");

        countryId = userApp.CountryId;
        ownerEmail = userApp.Email;
      }

      var retData = new InviteData {Email = invite.Email, CountryId = countryId, OwnerEmail = ownerEmail};

      return new ObjectResult(retData);
    }

    [HttpPost("invite")]
    [Authorize]
    public async Task<IActionResult> SetInvite([FromBody] Invite invite)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        InviteDbProvider.RemoveAllOldInvitations();
        var inviteRole = Roles.RolesPermissions.Find(f => f.Name == invite.Role);
        if (inviteRole == null) return NotFound("Роль не найдена!");

        var userRolePrm = Roles.RolesPermissions.Find(f => f.Name == user.Role);
        if (userRolePrm == null) return NotFound("Роль пользователя не найдена!");

        if (userRolePrm.Code < inviteRole.Code) return StatusCode(403, "Ваша роль не соответствует!");

        if (InviteDbProvider.CheckInvite(invite.Email)) return BadRequest("Уже есть приглашение на этот email!");
        if (await _userManager.FindByEmailAsync(invite.Email) != null) return BadRequest("Такой email уже используется!");

        var id = InviteDbProvider.AddInvite(new InviteRegistration {Email = invite.Email, Role = invite.Role, OwnerId = invite.OwnerId});

        if (id == null) return BadRequest("Ошибка добавления приглашения!");

        var host = HttpContext.Request.Host;
        var sch = HttpContext.Request.Scheme;
        var callbackUrl = $"{sch}://{host}/Account/Register?code={id}";
        var message = new StringBuilder();
        message.AppendLine("<div>Вас пригласили присоединиться с системе третий кран!</div><br />");
        message.AppendLine("<div>Для регистрации перейдите по ");
        try
        {
          await _emailSender.SendEmailAsync(invite.Email, "Приглашение от компании третий кран",
                                            $"{message} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>.</div>");

          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
            Message = $"Пользователь {user} отослал приглашение [{invite.Email}] на роль [{invite.Role}]"
          });
          return Ok();
        }
        catch (Exception ex)
        {
          InviteDbProvider.RemoveInvite(id);
          return BadRequest(ex.Message);
        }
      }
      catch (Exception ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("setInfos")]
    [Authorize]
    public async Task<IActionResult> SetInfos([FromBody] UserInfoAlert userEdit)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        string emails = null;
        if ((userEdit.InfoEmails != null) && (userEdit.InfoEmails.Length > 0))
        {
          var em = new StringBuilder();
          for (var i = 0; i < userEdit.InfoEmails.Length; i++)
          {
            if (userEdit.InfoEmails[i].ToUpper() == user.Email.ToUpper()) continue;

            em.Append(userEdit.InfoEmails[i]);
            if (i != userEdit.InfoEmails.Length - 1) em.Append(',');
          }

          emails = em.ToString();
        }

        user.InfoEmails = emails;

        var alerts = 0;
        if ((userEdit.UserAlerts != null) && (userEdit.UserAlerts.Length > 0))
        {
          foreach (var alert in userEdit.UserAlerts)
          {
            if (alert.Type == "NO_LINK")
            {
              if (alert.Active) alerts = alerts.SetBitTo1(0);
              else alerts = alerts.SetBitTo0(0);
            }

            if (alert.Type == "TANK_EMPTY")
            {
              if (alert.Active) alerts = alerts.SetBitTo1(1);
              else alerts = alerts.SetBitTo0(1);
            }

            if (alert.Type == "NO_SALES")
            {
              if (alert.Active) alerts = alerts.SetBitTo1(2);
              else alerts = alerts.SetBitTo0(2);
            }

            if (alert.Type == "REPORT")
            {
              if (alert.Active) alerts = alerts.SetBitTo1(3);
              else alerts = alerts.SetBitTo0(3);
            }
          }
        }

        user.UserAlerts = alerts;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          await UserDbProvider.AddLog(new LogUsr
          {
            UserId = user.Id, Email = user.Email, Phone = user.PhoneNumber, LogDate = DateTime.Now,
            Message = $"Пользователь {user} изменил данные рассылки оповещений"
          });
          return Ok(user);
        }

        return new BadRequestResult();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }
  }
}
