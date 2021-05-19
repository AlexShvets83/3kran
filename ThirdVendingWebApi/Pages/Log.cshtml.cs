using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ThirdVendingWebApi.Tools;

namespace ThirdVendingWebApi.Pages
{
  public class LogModel : PageModel
  {
    public void OnGet()
    {
      //var info = DeviceTool.WriteCommand("journalctl -u 3kran-web.service");
      ViewData["Message"] = null;
    }
  }
}
