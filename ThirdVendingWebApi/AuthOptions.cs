using DeviceDbModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace ThirdVendingWebApi
{
  /// <summary>
  ///   JWT authentication options
  /// </summary>
  public static class AuthOptions
  {
    private static readonly IConfiguration JwtSection;

    static AuthOptions()
    {
      IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(MainSettings.JsonPath).Build();
      JwtSection = config.GetSection("AuthOptions");
    }

    /// <summary>
    ///   издатель токена
    /// </summary>
    public static string JwtIssuer => JwtSection["JwtIssuer"];

    /// <summary>
    ///   потребитель токена
    /// </summary>
    public static string JwtAudience => JwtSection["JwtAudience"];

    /// <summary>
    ///   ключ для шифрования
    /// </summary>
    private static string JwtKey => JwtSection["JwtAudience"];

    /// <summary>
    ///   время жизни токена
    /// </summary>
    public static int JwtExpireMinutes => Convert.ToInt32(JwtSection["JwtExpireMinutes"]);

    /// <summary>
    ///   Get Symmetric Security Key
    /// </summary>
    /// <returns></returns>
    public static SymmetricSecurityKey GetSymmetricSecurityKey() { return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtKey)); }
  }
}
