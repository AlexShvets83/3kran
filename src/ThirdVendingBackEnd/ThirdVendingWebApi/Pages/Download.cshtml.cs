using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThirdVendingWebApi.Pages
{
  public class DownloadModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public DownloadModel(UserManager<ApplicationUser> userManager) { _userManager = userManager; }

    public async Task<IActionResult> OnGet(string id)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      var fileLink = FilesDbProvider.GetFile(id);

      //var mas = await System.IO.File.ReadAllBytesAsync(fileLink.Path);
      //return File(mas, fileLink.FileType, fileLink.Name);
      return PhysicalFile(fileLink.Path, fileLink.FileType, fileLink.Name);
    }
  }
}
