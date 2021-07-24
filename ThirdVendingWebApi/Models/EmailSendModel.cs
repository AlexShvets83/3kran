using System.Collections.Generic;

namespace ThirdVendingWebApi.Models
{
  public class EmailSendModel
  {
    public string EmailTheme { get; set; }

    public string EmailBody { get; set; }

    public List<string> Addressees { get; set; }
  }
}
