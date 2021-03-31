using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdVendingWebApi.Models
{
  /// <summary>
  /// Изменение пароля
  /// </summary>
  public class ChangePsw
  {
    /// <summary>
    /// Текущий пароль
    /// </summary>
    public string CurrentPassword { get; set; }
    
    /// <summary>
    /// Новый пароль
    /// </summary>
    public string NewPassword { get; set; }
  }
}
