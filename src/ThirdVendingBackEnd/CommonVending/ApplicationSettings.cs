using DeviceDbModel.Models;

namespace CommonVending
{
  public static class ApplicationSettings
  {
    public static int UserLogDepth { get; set; }

    public static bool SupportBoard1 { get; set; } = true;

    public static bool SupportBoard2 { get; set; } = true;

    public static bool SupportBoard3 { get; set; } = true;

    public static long FileMaxUploadLenght { get; set; }

    public static int UsersMaxDownloads { get; set; }

    public static void Set(AppSettings data)
    {
      if (data == null) return;

      UserLogDepth = data.UserLogDepth;
      SupportBoard1 = data.SupportBoard1;
      SupportBoard2 = data.SupportBoard2;
      SupportBoard3 = data.SupportBoard3;
      FileMaxUploadLenght = data.FileMaxUploadLenght;
      UsersMaxDownloads = data.UsersMaxDownloads;
    }
  }
}
