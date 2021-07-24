using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using CommonVending.DbProvider;
using DeviceDbModel.Models;
using ThirdVendingWebApi.Tools;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LogController : ControllerBase
  {
    [HttpGet("/api/log!1231easdda!11!2@")]
    public string Getlog()
    {
      var info = DeviceTool.WriteCommand("journalctl -u 3kran-web.service");
      var list = info.Split("\r\n");
      var db = new StringBuilder();
      Array.Reverse(list);

      foreach (var str in list) { db.AppendLine(str.Trim('\n')); }

      //var newList = list.Reverse();
      return db.ToString();
    }

    [HttpGet("getLog")]
    public List<LogUsr> GetUsersLog()
    {
      return UserDbProvider.GetUserLog();
    }
  }
}
