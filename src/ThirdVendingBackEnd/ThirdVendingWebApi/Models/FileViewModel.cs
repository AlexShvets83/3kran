using System;

namespace ThirdVendingWebApi.Models
{
  public class FileViewModel
  {
    public string Id { get; set; }

    public DateTime FileDate { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public string FileType { get; set; }
    
    public bool Visible { get; set; }
  }
}
