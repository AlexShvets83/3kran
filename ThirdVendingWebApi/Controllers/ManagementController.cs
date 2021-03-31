using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ThirdVendingWebApi.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  public class ManagementController : ControllerBase
  {
    /// <summary>
    /// for stupid front
    /// </summary>
    /// <returns></returns>
    [HttpGet("/management/info")]
    public async Task Info()
    {
      //var response = new {activeProfiles = [{"prod"}]};
      //activeProfiles: ["prod"]
     // 0: "prod"
      //display-ribbon-on-profiles: "dev"
      //mailEnabled: false

      // сериализация ответа
      Response.ContentType = "application/json";
      await Response.WriteAsync(JsonConvert.SerializeObject(null, new JsonSerializerSettings {Formatting = Formatting.Indented}));
    }
  }
}
