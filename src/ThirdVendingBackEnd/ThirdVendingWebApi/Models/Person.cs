using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ThirdVendingWebApi.Models
{
  /// <summary>
  ///   Credentials 
  /// </summary>
  [DataContract]
  public class Person
  {
    /// <summary>
    ///   User name
    /// </summary>
    [JsonPropertyName("username")]
    [JsonProperty("username")]
    public string UserName { get; set; }

    /// <summary>
    ///   Password
    /// </summary>
    [JsonPropertyName("password")]
    [JsonProperty("password")]
    public string Password { get; set; }
  }
}
