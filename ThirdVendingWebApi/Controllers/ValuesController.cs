using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdVendingWebApi.Tools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ControllerBase
  {
    // GET: api/<ValuesController>
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/<ValuesController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<ValuesController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<ValuesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ValuesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }

    [HttpGet("/api/log!1231easdda!11!2@")]
    public string Getlog()
    {
      var info = DeviceTool.WriteCommand("journalctl -u 3kran-web.service");
      var list = info.Split("\r\n");
      var db = new StringBuilder();
      Array.Reverse(list);

      foreach (var str in list)
      {
        db.AppendLine(str.Trim('\n'));
      }

      //var newList = list.Reverse();
      return db.ToString();
    }
  }
}
