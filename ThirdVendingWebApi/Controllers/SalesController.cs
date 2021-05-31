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
  public class SalesController : ControllerBase
  {
    [HttpGet("info")]

    //[Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      return await Task.Factory.StartNew(() =>
      {
        var sales = (from != null) && (to != null)
                      ? SalesDbProvider.GetDeviceSales(deviceId, from.Value, to.Value)
                      : SalesDbProvider.GetDeviceSales(deviceId, 5);
        var retList = sales.Select(Main.GetNewObj<DevSaleView>).ToList();
        return new ObjectResult(retList);
      });
    }
  }
}
