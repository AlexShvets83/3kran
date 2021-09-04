using System;

namespace ThirdVendingWebApi.Models.Device
{
  public class DeviceAlert
  {
    //public long Id { get; set; }

    //public string DeviceId { get; set; }

    //public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    //public double Timestamp { get; set; }

    public int CodeType { get; set; }

    public string Message { get; set; }
  }
}
