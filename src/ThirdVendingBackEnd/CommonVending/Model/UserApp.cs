using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonVending.Model
{
  public class UserApp
  {
    
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

    public string OwnerEmail { get; set; }

    public string AddDealerName { get; set; }

    public string Role { get; set; }

    public bool CommerceVisible { get; set; }

  }
}
