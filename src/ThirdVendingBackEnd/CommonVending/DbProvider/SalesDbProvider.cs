using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;

namespace CommonVending.DbProvider
{
  public class SalesDbProvider
  {
    static SalesDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
    
    public static DevSale GetLastSale(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceSales.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static List<DevSale> GetDeviceSales(string devId, int limit)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceSales.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(limit).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<DevSale> GetDeviceSales(string devId, DateTime from, DateTime to)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceSales.Where(w => (w.DeviceId == devId) && (w.MessageDate >= from) && (w.MessageDate <= to)).OrderByDescending(o => o.MessageDate).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static void InsertDeviceSale(DevSale record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.DeviceSales.Add(record);
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
