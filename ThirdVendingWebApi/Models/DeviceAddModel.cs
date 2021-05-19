using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Models
{
  public class DeviceAddModel
  {
    public string Id { get; set; }

    public string DeviceId { get; set; }

    public string OwnerId { get; set; }

    public string Address { get; set; }

    public int TimeZone { get; set; }

    public string Currency { get; set; }

    public string Phone { get; set; }
  }
}
