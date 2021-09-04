namespace DeviceDbModel.Models
{
  public class AppSettings
  {
    public int Id { get; set; }

    public int UserLogDepth { get; set; }

    public bool SupportBoard1 { get; set; }

    public bool SupportBoard2 { get; set; }

    public bool SupportBoard3 { get; set; }

    public long FileMaxUploadLenght { get; set; }

    public int UsersMaxDownloads { get; set; }
  }
}
