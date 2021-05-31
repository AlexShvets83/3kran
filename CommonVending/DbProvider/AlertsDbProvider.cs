using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;

namespace CommonVending.DbProvider
{
  public static class AlertsDbProvider
  {
    static AlertsDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }

    public static void InsertDeviceAlert(DevAlert record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.DeviceAlerts.Add(record);
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static DevAlert GetDeviceLastAlert(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceAlerts.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<DevAlert> GetDeviceAlert(string devId, int limit)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceAlerts.Where(w => (w.DeviceId == devId) & (w.CodeType < 1)).OrderByDescending(o => o.MessageDate).Take(limit).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<DevAlert> GetDeviceAlertEvent(string devId, DateTime from, DateTime to)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceAlerts.Where(w => (w.DeviceId == devId) && (w.MessageDate >= from) && (w.MessageDate <= to)).OrderByDescending(o => o.MessageDate).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static DevAlert GetLastConnAlert(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceAlerts.Where(w => (w.DeviceId == devId) && ((w.CodeType == -1) || (w.CodeType == 1))).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static DevAlert GetLastSaleAlert(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceAlerts.Where(w => (w.DeviceId == devId) && ((w.CodeType == 0) || (w.CodeType == 3))).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
  }
}
