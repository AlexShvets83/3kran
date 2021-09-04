using System.IO;
using DeviceDbModel;
using Microsoft.Extensions.Configuration;

namespace CommonVending.Services
{
  public class SmtpClientSettings
  {
    private readonly IConfiguration SmtpSection;

    public SmtpClientSettings()
    {
      IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(MainSettings.JsonPath).Build();
      SmtpSection = config.GetSection("SmtpClientSettings");
    }

    public string Host => SmtpSection["Host"];

    public int Port => int.Parse(SmtpSection["Port"]);

    public bool EnableSsl => bool.Parse(SmtpSection["EnableSsl"]);

    public string UserName => SmtpSection["UserName"];

    public string Password => SmtpSection["Password"];
  }
}
