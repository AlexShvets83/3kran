using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ThirdVendingWebApi.Models.Users
{
  public class UserInfoAlert
  {
    /// <summary>
    ///   Массив имейлов для рассылки писем с авариями и событиями
    /// </summary>
    public string[] InfoEmails { get; set; }

    /// <summary>
    ///   To DB
    /// </summary>
    public AlertTypes[] UserAlerts { get; set; }
  }
}
