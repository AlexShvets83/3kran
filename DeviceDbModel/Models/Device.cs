using System.Collections.Generic;

namespace DeviceDbModel.Models
{
  public class Device
  {
    public Device() { UserDevicePermissions = new HashSet<UserDevicePermission>(); }

    public string Id { get; set; }

    public string DeviceId { get; set; }

    public string OwnerId { get; set; }

    public string Address { get; set; }

    public int TimeZone { get; set; }

    public string Currency { get; set; }

    public string Phone { get; set; }

    public virtual ApplicationUser User { get; set; }

    public ICollection<UserDevicePermission> UserDevicePermissions { get; set; }
  }
}
