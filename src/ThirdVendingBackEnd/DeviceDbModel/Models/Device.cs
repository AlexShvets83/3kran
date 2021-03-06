using System.Collections.Generic;

namespace DeviceDbModel.Models
{
  public class Device
  {
    public Device()
    {
      UserDevicePermissions = new HashSet<UserDevicePermission>();
      DevStatuses = new HashSet<DevStatus>();
      DevErrorStatuses = new HashSet<DevErrorStatus>();
      DevSales = new HashSet<DevSale>();
      DevEncashes = new HashSet<DevEncash>();
      DevAlerts = new HashSet<DevAlert>();
      DevInfos = new HashSet<DevInfo>();
      DevSettings = new HashSet<DevSetting>();
    }

    public string Id { get; set; }

    public string Imei { get; set; }

    public string OwnerId { get; set; }

    public string Address { get; set; }

    public int TimeZone { get; set; }

    public string Currency { get; set; }

    public string Phone { get; set; }

    public virtual ApplicationUser User { get; set; }

    public ICollection<UserDevicePermission> UserDevicePermissions { get; set; }

    public ICollection<DevStatus> DevStatuses { get; set; }

    public ICollection<DevErrorStatus> DevErrorStatuses { get; set; }

    public ICollection<DevSale> DevSales { get; set; }

    public ICollection<DevEncash> DevEncashes { get; set; }

    public ICollection<DevAlert> DevAlerts { get; set; }

    public ICollection<DevInfo> DevInfos { get; set; }

    public ICollection<DevSetting> DevSettings { get; set; }
  }
}
