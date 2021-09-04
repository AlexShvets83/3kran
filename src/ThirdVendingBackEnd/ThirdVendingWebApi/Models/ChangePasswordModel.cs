using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Models
{
  public class ChangePasswordModel
  {
    public string Key { get; set; }

    public string NewPassword { get; set; }
  }
}
