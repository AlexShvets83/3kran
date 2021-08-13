using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using Microsoft.EntityFrameworkCore;

namespace CommonVending.DbProvider
{
  public class ToolsDbProvider
  {
    static ToolsDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }

    public static DateTime? GetMinDateData()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var rA = context.DeviceAlerts.Min(d => d.MessageDate);
          var minD = rA;
          var rE = context.DeviceEncashes.Min(d => d.MessageDate);
          if (minD > rE) minD = rE;
          var rS = context.DeviceSales.Min(d => d.MessageDate);
          if (minD > rS) minD = rS;

          return minD;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static int DeleteDataBetweenDate(DateTime startDate, DateTime endDate)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var numberOfRowDeleted = context.Database.ExecuteSqlRaw($"DELETE FROM device_alerts WHERE message_date BETWEEN '{startDate:yyyy-MM-dd}' AND  '{endDate:yyyy-MM-dd}'");
          numberOfRowDeleted += context.Database.ExecuteSqlRaw($"DELETE FROM device_sales WHERE message_date BETWEEN '{startDate:yyyy-MM-dd}' AND  '{endDate:yyyy-MM-dd}'");
          numberOfRowDeleted += context.Database.ExecuteSqlRaw($"DELETE FROM device_encashes WHERE message_date BETWEEN '{startDate:yyyy-MM-dd}' AND  '{endDate:yyyy-MM-dd}'");
          return numberOfRowDeleted;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return 0;
      }
    }
  }
}
