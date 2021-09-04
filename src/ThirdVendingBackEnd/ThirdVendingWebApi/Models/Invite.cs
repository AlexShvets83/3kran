using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Models
{
  public class Invite
  {
    public string OwnerId { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
  }
}
