using CommonVending;
using CommonVending.DbProvider;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ThirdVendingWebApi.Models.Device;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AlertsController : ControllerBase
  {
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      return await Task.Factory.StartNew(() =>
      {
        var alerts = (from != null) && (to != null) 
                       ? AlertsDbProvider.GetDeviceAlertEvent(deviceId, from.Value, to.Value) 
                       : AlertsDbProvider.GetDeviceAlert(deviceId, 5);
        var retList = alerts?.Select(Main.GetNewObj<DeviceAlert>).ToList();
        return new ObjectResult(retList);
      });
    }
  }
}
