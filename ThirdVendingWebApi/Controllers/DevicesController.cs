using CommonVending;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class DevicesController : ControllerBase
  {
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    public DevicesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
      _userManager = userManager;
      _signInManager = signInManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(int size)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound();
      if (!user.Activated) return NotFound();

      return new ObjectResult(null);
    }
  }
}
