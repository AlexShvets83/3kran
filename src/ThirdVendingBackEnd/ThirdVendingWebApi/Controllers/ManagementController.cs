using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
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
    /// for front
    /// </summary>
    /// <returns></returns>
    [HttpGet("/management/info")]
    public async Task Info()
    {
      var mg = new ManagementInfo();
      Response.ContentType = "application/json";
      await Response.WriteAsync(JsonConvert.SerializeObject(mg, new JsonSerializerSettings {Formatting = Formatting.Indented}));
    }
  }

  public class ManagementInfo
  {
    [JsonPropertyName("display-ribbon-on-profiles")]
    [JsonProperty("display-ribbon-on-profiles")]
    public string DisplayRibbonOnProfiles => "dev";

    [JsonPropertyName("activeProfiles")]
    [JsonProperty("activeProfiles")]
    public string[] ActiveProfiles => new[] {"prod"};

    [JsonPropertyName("mailEnabled")]
    [JsonProperty("mailEnabled")]
    public bool MailEnabled => false;
  }
}
