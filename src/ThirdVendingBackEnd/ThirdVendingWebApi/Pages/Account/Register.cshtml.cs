using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThirdVendingWebApi.Pages.Account
{
  [AllowAnonymous]
  public class RegisterModel : PageModel
  {
    ////[BindProperty]
    //[BindProperty(SupportsGet = true)]
    //public InputModel Input { get; set; }

    //public string ReturnUrl { get; set; }

    //public async Task OnGetAsync(string returnUrl = null)
    //{
    //  Input = new InputModel();
    //  ReturnUrl = returnUrl;
    //}

    //#region Nested types

    //public class InputModel
    //{
    //  [Required]
    //  [EmailAddress]
    //  [Display(Name = "Email")]
    //  public string Email { get; set; }

    //  [Required]
    //  [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    //  [DataType(DataType.Password)]
    //  [Display(Name = "Password")]
    //  public string Password { get; set; }

    //  [DataType(DataType.Password)]
    //  [Display(Name = "Confirm password")]
    //  [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //  public string ConfirmPassword { get; set; }
    //}

    //#endregion

    //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    //{
    //    returnUrl ??= Url.Content("~/");
    //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    //    if (ModelState.IsValid)
    //    {
    //        var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
    //        var result = await _userManager.CreateAsync(user, Input.Password);
    //        if (result.Succeeded)
    //        {
    //            _logger.LogInformation("User created a new account with password.");

    //            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    //            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
    //            var callbackUrl = Url.Page(
    //                "/Account/ConfirmEmail",
    //                pageHandler: null,
    //                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
    //                protocol: Request.Scheme);

    //            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
    //                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

    //            if (_userManager.Options.SignIn.RequireConfirmedAccount)
    //            {
    //                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
    //            }
    //            else
    //            {
    //                await _signInManager.SignInAsync(user, isPersistent: false);
    //                return LocalRedirect(returnUrl);
    //            }
    //        }
    //        foreach (var error in result.Errors)
    //        {
    //            ModelState.AddModelError(string.Empty, error.Description);
    //        }
    //    }

    //    // If we got this far, something failed, redisplay form
    //    return Page();
    //}
  }
}
