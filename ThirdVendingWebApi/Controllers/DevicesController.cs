using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CommonVending.DbProvider;

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
    /// <returns></returns>
    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> Get()
    {
      var devList = DeviceDbProvider.GetDevices();
      //todo add last state
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //if (user == null) return NotFound();
      //if (!user.Activated) return NotFound();

      //var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
      //Response.Headers.Add("Link", headerStr);
      //Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(devList);
    }

    [HttpGet("/api/_search/devices")]
    [Authorize]
    public async Task<IActionResult> GetDevices(string query, int page, int size, string sort) //Get(int size)
    {
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //if (user == null) return NotFound();
      //if (!user.Activated) return NotFound();
      //http: //95.183.10.198/api/_search/devices?query=f2f196db-4cb1-47a4-9b74-2f602c2c5517&page=0&size=10&sort=id.keyword,desc
      var headerStr = $@"</api/devices?page=1&size={size}>; rel=""next"",</api/devices?page=1&size={size}>; rel=""last"",</api/devices?page=0&size={size}>; rel=""first""";
      Response.Headers.Add("Link", headerStr);
      Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(new string [0]);
    }
  }
}
