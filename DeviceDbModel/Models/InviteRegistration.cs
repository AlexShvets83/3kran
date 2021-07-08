using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDbModel.Models
{
  public class InviteRegistration
  {
    public string Id { get; set; }

    public string OwnerId { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual ApplicationUser User { get; set; }
  }
}
