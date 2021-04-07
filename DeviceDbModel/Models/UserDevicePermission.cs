namespace DeviceDbModel.Models
{
  public class UserDevicePermission
  {
    public string Id { get; set; }

    public string UserId { get; set; }

    public string DeviceId { get; set; }

    public bool CommerceVisible { get; set; }

    public bool TechEditable { get; set; }

    public virtual ApplicationUser User { get; set; }

    public Device Device { get; set; }
  }
}
