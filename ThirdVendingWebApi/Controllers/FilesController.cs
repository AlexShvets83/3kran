using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class FilesController : ControllerBase
  {
    [HttpGet]
    [Authorize]
    [Produces(typeof(List<FileModel>))]
    public async Task<IActionResult> Get(int size)
    {
      //Link: </api/files?page=0&size=1000>; rel="last",</api/files?page=0&size=1000>; rel="first"
      var headerStr = $@"</api/files?page=1&size={size}>; rel=""last"",</api/files?page=0&size={size}>; rel=""first""";
      Response.Headers.Add("Link", headerStr);
      Response.Headers.Add("X-Total-Count", "0");
      return new ObjectResult(new FileModel[0]);
    }
  }
}
