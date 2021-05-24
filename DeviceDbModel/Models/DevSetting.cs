using System;

namespace DeviceDbModel.Models
{
  public class DevSetting
  {
    public int Id { get; set; }

    public string DeviceId { get; set; }

    public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    public string Topic { get; set; }

    public string Payload { get; set; }

    public string Md5 { get; set; }

    public Device Device { get; set; }
  }
}
