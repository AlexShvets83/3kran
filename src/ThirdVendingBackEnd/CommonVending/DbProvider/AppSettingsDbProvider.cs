using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;

namespace CommonVending.DbProvider
{
  public class AppSettingsDbProvider
  {
    static AppSettingsDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }

    public static AppSettings GetAppSettings()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var settingsList = context.AppSettings.ToList();
          var appSet = settingsList[0];
          ApplicationSettings.Set(appSet);

          return settingsList[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static async Task SetAppSettings(AppSettings record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.AppSettings.Update(record);
          await context.SaveChangesAsync();

          ApplicationSettings.Set(record);
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
