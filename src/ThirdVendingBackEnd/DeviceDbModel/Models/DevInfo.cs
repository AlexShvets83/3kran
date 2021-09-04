using System;

namespace DeviceDbModel.Models
{
  public class DevInfo
  {
    public int Id { get; set; }

    public string DeviceId { get; set; }

    //public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    public string Name { get; set; }

    public float Value { get; set; }

    public Device Device { get; set; }
  }
}
