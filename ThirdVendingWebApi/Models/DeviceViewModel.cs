using DeviceDbModel.Models;
using ThirdVendingWebApi.Models.Device;

namespace ThirdVendingWebApi.Models
{
  public class DeviceViewModel
  {
    public string Id { get; set; }

    public string Imei { get; set; }

    public string OwnerId { get; set; }

    public string Address { get; set; }

    public int TimeZone { get; set; }

    public string Currency { get; set; }

    public string Phone { get; set; }

    public string OwnerName { get; set; }

    public string OwnerEmail { get; set; }

    public DeviceStatus LastStatus { get; set; }

    public DevSale LastSale { get; set; }

    public DevAlert LastAlert { get; set; }
  }
}
