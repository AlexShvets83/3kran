namespace DeviceDbModel
{
  public class JsonSettings
  {
    public Connectionstrings ConnectionStrings { get; set; }
    public SmtpClientSettings SmtpClientSettings { get; set; }
    public Authoptions AuthOptions { get; set; }
  }

  public class Connectionstrings
  {
    //private readonly string dfc = "Host=95.183.10.198;Port=5432;Database=devicedb;Username=kran;Password={0}";

    public string DefaultConnection
    {
      get
      {
        var host = "Host=localhost;";
#if DEBUG
        host = "Host=95.183.10.198;";
#endif
        host += $"Port=5432;Database=devicedb;Username=kran;Password={Password}";
        return host;
      }
    }

    public string Password { get; set; } = "qwerty";
  }

  public class SmtpClientSettings
  {
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
  }

  public class Authoptions
  {
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
    public string JwtKey { get; set; }
    public string JwtExpireMinutes { get; set; }
  }
}
