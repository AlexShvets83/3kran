using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace DeviceDbModel.Models
{
  /// <inheritdoc />
  public class ApplicationUser : IdentityUser
  {
    public ApplicationUser()
    {
      Slaves = new HashSet<ApplicationUser>();
      Devices = new HashSet<Device>();
      UserDevicePermissions = new HashSet<UserDevicePermission>();
      InviteRegistrations = new HashSet<InviteRegistration>();
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Patronymic { get; set; }

    public string Organization { get; set; }

    public string City { get; set; }

    public string ImageUrl { get; set; }

    public string InfoEmails { get; set; }

    public bool Activated { get; set; }

    public string LangKey { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public int UserAlerts { get; set; }

    public string CountryId { get; set; }

    public Country Country { get; set; }

    public string OwnerId { get; set; }

    public string InviteCode { get; set; }

    public ApplicationUser Master { get; set; }

    public ICollection<ApplicationUser> Slaves { get; set; }

    public ICollection<Device> Devices { get; set; }

    public ICollection<UserDevicePermission> UserDevicePermissions { get; set; }

    public ICollection<InviteRegistration> InviteRegistrations { get; set; }
  }
}
