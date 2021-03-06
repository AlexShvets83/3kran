using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceDbModel.Models
{
  /// <inheritdoc />
  public class ApplicationUser : IdentityUser
  {
    public ApplicationUser()
    {
      Сustomers = new HashSet<ApplicationUser>();
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

    public bool? Activated { get; set; }

    public string LangKey { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime LastModifiedDate { get; set; }

    public int UserAlerts { get; set; }

    public int CountryId { get; set; }

    public string OwnerId { get; set; }

    public string AddDealerName { get; set; }

    public string Role { get; set; }

    public bool CommerceVisible { get; set; }

    public Country Country { get; set; }

    public ApplicationUser Owner { get; set; }

    public ICollection<ApplicationUser> Сustomers { get; set; }

    public ICollection<Device> Devices { get; set; }

    public ICollection<UserDevicePermission> UserDevicePermissions { get; set; }

    public ICollection<InviteRegistration> InviteRegistrations { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      return $"[{Email}] {FirstName} {Patronymic} {LastName}";
    }
  }
}
