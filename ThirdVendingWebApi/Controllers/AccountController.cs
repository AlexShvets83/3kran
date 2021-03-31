using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CommonVending;
using CommonVending.Crypt;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.WebUtilities;
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
      var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == person.UserName);
      if (appUser != null)
      {
        var result = await _signInManager.CheckPasswordSignInAsync(appUser, person.Password, false);
        if (result.Succeeded)
        {
          var now = DateTime.UtcNow;
          var claims = new List<Claim>
          {
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, appUser.Id)
          };

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

      userApp.CopyObjectProperties(user);

      //todo ROLES

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
      //var resp = new
      //{
      //  entityName = "userManagement", errorKey = "userexists", type = "https://www.jhipster.tech/problem/login-already-used", title = "Login name already used!",
      //  status = 400, message = "error.userexists", "params" = "userManagement"
      //};
      var retBad = @"{""entityName"":""userManagement"",""errorKey"":""userexists"",""type"":""https://www.jhipster.tech/problem/login-already-used"",""title"":""Login name already used!"",""status"":400,""message"":""error.userexists"",""params"":""userManagement""}";
      var userCheck = await _userManager.FindByEmailAsync(user.Email);
      if (userCheck != null)
      {
        return BadRequest(retBad);
      }

      userCheck = await _userManager.FindByNameAsync(user.UserName);
      if (userCheck != null)
      {
        return BadRequest(retBad);
      }

      //var userApp1 = await _userManager.GetUserAsync(HttpContext.User);
      //if ((userApp == null) || (userApp.UserName != user.UserName)) return;
      var userApp = new ApplicationUser();
      userApp.CopyObjectProperties(user);
      userApp.Activated = false;
      userApp.CreatedBy = "anonymousUser";
      userApp.CreatedDate = DateTime.Now;
      userApp.UserAlerts = 15;

      var result = await _userManager.CreateAsync(userApp, user.Password);
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(userApp, "ROLE_USER");
        return Ok();
      }

      return BadRequest(result.Errors);
    }

    [HttpPost("reset-password/init")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(string email)
    {
      var user = await _userManager.FindByEmailAsync(email);

      if (user == null)
      {
        var resp = new {type = "https://www.jhipster.tech/problem/email-not-found", title = "Email address not registered", status = 400};
        return BadRequest(resp);
      }

      try
      {
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
        message.AppendLine($"Уважаемый {user.FirstName} {user.Patronymic}!");
        message.AppendLine("Для вашего аккаунта в системе мониторинга «Третий кран» был запрошен сброс пароля.Чтобы сбросить пароль нажмите на");
        var callbackUrl = $"{sch}://{host}/#/reset/finish?key={ecode}";
        await _emailSender.SendEmailAsync(email, "Запрос на сброс пароля", 
                                          $"{message} \r\n \r\n <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылку</a>.");
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
    public async Task<IActionResult> ResetPasswordFinish(string key, string newPassword)
    {
      var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(key));
      var id = Crypto.Decrypt(code);
      var user = await _userManager.FindByIdAsync(id);
      if (user == null)
      {
        return BadRequest();
      }

      var passwordValidator = 
        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
      var passwordHasher =
        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;
     
      var result = await passwordValidator.ValidateAsync(_userManager, user, newPassword);
      if(result.Succeeded)
      {
        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
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

    //reset-password/init

    // PUT api/<ValuesController>/5
    [HttpPut("{id}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public void Put(int id, [FromBody] string value) { }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public void Delete(int id) { }
  }
}
