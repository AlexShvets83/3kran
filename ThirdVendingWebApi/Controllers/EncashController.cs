using CommonVending;
using CommonVending.DbProvider;
using CommonVending.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EncashController : ControllerBase
  {
    [HttpGet]

    //[Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      return await Task.Factory.StartNew(() =>
      {
        var encashes = (from != null) && (to != null)
                         ? DeviceDbProvider.GetDeviceEncash(deviceId, from.Value, to.Value)
                         : DeviceDbProvider.GetDeviceEncash(deviceId, 5);
        var retList = encashes.Select(Main.GetNewObj<DevEncashView>).ToList();
        return new ObjectResult(retList);
      });
    }
  }
}
