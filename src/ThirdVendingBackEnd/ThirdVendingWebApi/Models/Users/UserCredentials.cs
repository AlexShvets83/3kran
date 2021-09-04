using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ThirdVendingWebApi.Models.Users
{
  public class UserCredentials
  {
    /// <summary>
    ///   Email, электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// </summary>
    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///   Текущий пароль
    /// </summary>
    public string CurrentPassword { get; set; }

    /// <summary>
    ///   Новый пароль
    /// </summary>
    public string NewPassword { get; set; }
  }
}
