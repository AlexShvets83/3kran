using System.IO;
using Newtonsoft.Json;

namespace DeviceDbModel
{
  public static class MainSettings
  {
    public static string JsonPath => Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).ToString(), "main.json");

    public static JsonSettings Settings { get; set; }

    public static void ReloadSettings()
    {
      Settings = JsonConvert.DeserializeObject<JsonSettings>(File.ReadAllText(JsonPath));
    }

    static MainSettings() { ReloadSettings(); }
  }
}
