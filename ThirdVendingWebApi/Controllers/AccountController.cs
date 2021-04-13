using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CommonVending;
using CommonVending.Crypt;
using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ThirdVendingWebApi.Models;

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
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "userManager"></param>
    /// <param name = "signInManager"></param>
    /// <param name = "emailSender"></param>
    /// <param name = "roleManager"></param>
    /// <param name = "configuration"></param>
    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             IEmailSender emailSender,
                             RoleManager<IdentityRole> roleManager,
                             IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _emailSender = emailSender;
      _roleManager = roleManager;
      _configuration = configuration;
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
        var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == person.UserName) ??
                      _userManager.Users.SingleOrDefault(r => r.NormalizedEmail == person.UserName.ToUpper()) ??
                      _userManager.Users.SingleOrDefault(r => r.PhoneNumber == person.UserName);

        if (appUser != null)
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
      if (!user.Activated) return NotFound();

      var retUser = user.GetNewObj<UserAccount>();

      var roles = await _userManager.GetRolesAsync(user);

      retUser.Authorities = roles.ToArray();

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
    public async Task Post([FromBody] UserAccountEdit user)
    {
      var userApp = await _userManager.GetUserAsync(HttpContext.User);
      if ((userApp == null) || (userApp.UserName != user.UserName)) return;

      //userApp.CopyObjectProperties(user);

      //without roles
      userApp.LastModifiedBy = userApp.UserName;
      userApp.LastModifiedDate = DateTime.Now;

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

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpPost("Change-password")]
    [Authorize]
    public async Task ChangePassword([FromBody] ChangePsw pws)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return;
      if (!user.Activated) return;

      await _userManager.ChangePasswordAsync(user, pws.CurrentPassword, pws.NewPassword);
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

        InviteRegistration invite = null;
        if (!string.IsNullOrEmpty(user.InviteCode))
        {
          var owner = _userManager.Users.FirstOrDefault(f => f.Id == user.InviteCode);
          if (owner == null) return NotFound("Пользователь с таким кодом приглашения не найден!");

          //get invite
          invite = DeviceDbProvider.GetInvite(user.InviteCode, user.Email);
          if (invite == null) return NotFound("Код приглашения недействителен!");

          user.CountryId = owner.CountryId;
          user.OwnerId = owner.Id;
        }

        //var userApp1 = await _userManager.GetUserAsync(HttpContext.User);
        //if ((userApp == null) || (userApp.UserName != user.UserName)) return;
        var userApp = new ApplicationUser();
        userApp.CopyObjectProperties(user);
        userApp.Activated = false;
        userApp.CreatedBy = user.UserName;
        userApp.CreatedDate = DateTime.Now;
        userApp.UserAlerts = 15;

        userApp.CountryId = user.CountryId;
        userApp.OwnerId = user.OwnerId;

        var result = await _userManager.CreateAsync(userApp, user.Password);
        if (result.Succeeded)
        {
          //check role
          if ((invite != null) && (await _roleManager.FindByNameAsync(invite.Role) == null)) invite.Role = Roles.Owner;
          await _userManager.AddToRoleAsync(userApp, invite != null ? invite.Role : Roles.Owner);
          
          //delete invate
          if (invite != null) DeviceDbProvider.RemoveIvite(invite.Id);
          return Ok();
        }

        return BadRequest(result.Errors);
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpPost("reset-password/init")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword()
    {
      try
      {
        string email;
        using (var bodyStream = new StreamReader(Request.Body)) { email = await bodyStream.ReadToEndAsync(); }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
          var resp = new {type = "https://www.jhipster.tech/problem/email-not-found", title = "Email address not registered", status = 400};
          return BadRequest(resp);
        }

        var code = Crypto.Encrypt(user.Id);
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
        var callbackUrl = $"{sch}://{host}/#/reset/finish?key={ecode}";
        await _emailSender.SendEmailAsync(email, "Запрос на сброс пароля", $"{message} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылку</a>.</div>");
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
      var id = Crypto.Decrypt(code);
      var user = await _userManager.FindByIdAsync(id);
      if (user == null) { return BadRequest(); }

      var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
      var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

      var result = await passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
      if (result.Succeeded)
      {
        user.PasswordHash = passwordHasher.HashPassword(user, model.NewPassword);
        await _userManager.UpdateAsync(user);
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

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, [FromBody] string value)
    {
      //check admin
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Invalid ADMIN account!");

      //redundant check
      if (!admin.Activated) return BadRequest("Invalid ADMIN activation!");

      //check admin role
      var adminRoles = await _userManager.GetRolesAsync(admin);
      if (!adminRoles.Contains(Roles.Admin)) return Forbid();

      return Ok();
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> Delete(int id)
    {
      //check admin
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Invalid ADMIN account!");
      if (!admin.Activated) return BadRequest("Invalid ADMIN activation!");

      //check admin role
      var adminRoles = await _userManager.GetRolesAsync(admin);
      if (!adminRoles.Contains(Roles.Admin)) return Forbid();

      return Ok();
    }
  }
}
