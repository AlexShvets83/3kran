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
    private static readonly IConfiguration jwtSection;

    static AuthOptions()
    {
      IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(MainSettings.JsonPath).Build();
      jwtSection = config.GetSection("AuthOptions");
    }

    /// <summary>
    ///   издатель токена
    /// </summary>
    public static string JwtIssuer => jwtSection["JwtIssuer"];

    /// <summary>
    ///   потребитель токена
    /// </summary>
    public static string JwtAudience => jwtSection["JwtAudience"];

    /// <summary>
    ///   ключ для шифрования
    /// </summary>
    private static string JwtKey => jwtSection["JwtAudience"];

    /// <summary>
    ///   время жизни токена
    /// </summary>
    public static int JwtExpireMinutes => Convert.ToInt32(jwtSection["JwtExpireMinutes"]);

    /// <summary>
    ///   Get Symmetric Security Key
    /// </summary>
    /// <returns></returns>
    public static SymmetricSecurityKey GetSymmetricSecurityKey() { return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtKey)); }
  }
}
