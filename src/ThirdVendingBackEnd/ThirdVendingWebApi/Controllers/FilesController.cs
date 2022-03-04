using CommonVending;
using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ThirdVendingWebApi.Models;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class FilesController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public FilesController(UserManager<ApplicationUser> userManager) { _userManager = userManager; }

    [HttpGet]
    [Authorize]
    [Produces(typeof(List<FileViewModel>))]
    public async Task<IActionResult> Get(int size)
    {
      var admin = await _userManager.GetUserAsync(HttpContext.User);
      if (admin == null) return NotFound("Пользователь не найден!");
      if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      List<FileModel> fileList;
      if ((admin.Role == Roles.SuperAdmin) || (admin.Role == Roles.Admin)) fileList = FilesDbProvider.GetFiles();
      else fileList = FilesDbProvider.GetVisibleFiles();

      var retList = fileList.Select(Main.GetNewObj<FileViewModel>).ToList();
      return new ObjectResult(retList);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [Produces(typeof(List<FileViewModel>))]
    public async Task<IActionResult> GetFile(string id)
    {
      return await Task.Factory.StartNew(() =>
      {
        var fileLink = FilesDbProvider.GetFile(id);
        return PhysicalFile(fileLink.Path, fileLink.FileType, fileLink.Name);
      });
    }
    
    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
    [RequestSizeLimit(long.MaxValue)]
    [Authorize]
    public async Task<IActionResult> Post()
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено загружать файлы!");

        var file = Request.Form.Files[0];

        var folderName = "download_files";
        var pathToSave = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), folderName);
        if (file.Length > 0)
        {
          var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
          var fullPath = Path.Combine(pathToSave, fileName);
          using (var stream = new FileStream(fullPath, FileMode.Create)) { await file.CopyToAsync(stream); }

          var fileModel = new FileModel
          {
            Id = Guid.NewGuid().ToString(), FileDate = DateTime.Now, Name = file.FileName, Description = file.FileName,
            Size = file.Length, FileType = file.ContentType, Visible = false, Path = fullPath
          };

          var result = await FilesDbProvider.AddFile(fileModel);
          if (!result) return BadRequest();

          //await UserDbProvider.AddLog(new LogUsr
          //{
          //  UserId = admin.Id,
          //  Email = admin.Email,
          //  Phone = admin.PhoneNumber,
          //  LogDate = DateTime.Now,
          //  Message = $"Пользователь {admin} добавил файл {} на сервер"
          //});

          var files = new {name = fileName, size = file.Length};
          return Ok(new {files});
        }

        return BadRequest();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(string id, [FromBody] string dsc)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено изменять файлы!");

        await FilesDbProvider.SetFileDescription(id, dsc);

        return Ok(id);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpPut("setVisible/{id}")]
    [Authorize]
    public async Task<IActionResult> SetVisible(string id, [FromBody] bool visible)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено изменять файлы!");

        await FilesDbProvider.SetFileVisible(id, visible);

        return Ok(id);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
      try
      {
        var admin = await _userManager.GetUserAsync(HttpContext.User);
        if (admin == null) return NotFound("Пользователь не найден!");
        if (!admin.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");
        if ((admin.Role != Roles.SuperAdmin) && (admin.Role != Roles.Admin)) return StatusCode(403, "Вам запрещено изменять файлы!");

        var fileLink = FilesDbProvider.GetFile(id);
        
        //delete file physically 
        System.IO.File.Delete(fileLink.Path);

        //delete file link from dtabase 
        FilesDbProvider.RemoveFile(id);
        
        return Ok(id);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, $"Internal server error: {ex}");
      }
    }
  }
}
