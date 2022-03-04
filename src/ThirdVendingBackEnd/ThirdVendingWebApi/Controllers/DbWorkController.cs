using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using ThirdVendingWebApi.Tools;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class DbWorkController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public DbWorkController(UserManager<ApplicationUser> userManager) { _userManager = userManager; }
    
    [HttpGet]
    public async Task<IActionResult> Get(int size)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if (admin.Role != Roles.SuperAdmin) return StatusCode(403, "Нет прав!");
        
        var fileList = Directory.GetFiles("/home/3kran/backup");
        var retList = fileList.Select(Path.GetFileName).ToList();
        retList.Sort();
        return new ObjectResult(retList);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if (admin.Role != Roles.SuperAdmin) return StatusCode(403, "Нет прав!");

        var now = DateTime.Now;

        var fileName = $"Db_{now:yyyy-MM-dd}.back";
        var cmd = $"cd /home/3kran/backup/ && sudo -u postgres pg_dump -U postgres -w --format=custom devicedb > /home/3kran/backup/{fileName}";
        DeviceTool.WriteCommand(cmd);
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] string file)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if (admin.Role != Roles.SuperAdmin) return StatusCode(403, "Нет прав!");

        var cmd = $"cd /home/3kran/backup/ && sudo -u postgres pg_restore -U postgres -w -F custom -d devicedb -1 -c /home/3kran/backup/{file}";
        DeviceTool.WriteCommand(cmd);
        return Ok();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpGet("getDate")]
    public async Task<IActionResult> GetDate()
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if (admin.Role != Roles.SuperAdmin) return StatusCode(403, "Нет прав!");

        var endDate = DateTime.Now.AddYears(-1).Date;
        var strDate = ToolsDbProvider.GetMinDateData();

        if (strDate == null) return NotFound("Нет старых данных,которые можно удалить!");
        if (strDate > endDate) return NotFound("Нет старых данных,которые можно удалить!");

        var startDate = new DateTime(strDate.Value.Year, strDate.Value.Month, strDate.Value.Day);
        var response = new {startDate, endDate};
        return Ok(response);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(DateTime startDate, DateTime endDate)
    {
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      if (admin.Role != Roles.SuperAdmin) return StatusCode(403, "Нет прав!");

      if (startDate >= endDate) return NotFound("Начальная дата больше либо равна конечной");

      var nmb = ToolsDbProvider.DeleteDataBetweenDate(startDate, endDate);
      return Ok(nmb);
    }
  }
}
