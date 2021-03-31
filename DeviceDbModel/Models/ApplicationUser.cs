using Microsoft.AspNetCore.Identity;
using System;

namespace DeviceDbModel.Models
{
  /// <inheritdoc />
  public class ApplicationUser : IdentityUser
  {
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
  }
}
