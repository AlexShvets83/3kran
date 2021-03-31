using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Models
{
  public class FileModel
  {
    public string Id { get; set; }

    public long Timestamp { get; set; }

    public string Path { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public string Mime { get; set; }

    public string Md5 { get; set; }

    public bool Visible { get; set; }
  }
}
