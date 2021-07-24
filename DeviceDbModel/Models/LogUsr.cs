using System;

namespace DeviceDbModel.Models
{
  public class LogUsr
  {
    public long Id { get; set; }

    public DateTime LogDate { get; set; }

    public string UserId { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Imei { get; set; }

    public string Message { get; set; }
  }
}
