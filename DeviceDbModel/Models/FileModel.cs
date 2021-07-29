using System;

namespace DeviceDbModel.Models
{
  public class FileModel
  {
    public string Id { get; set; }

    public DateTime FileDate { get; set; }

    public string Path { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public long Size { get; set; }

    public string FileType { get; set; }
    
    public bool Visible { get; set; }
  }
}
