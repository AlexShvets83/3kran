using System;
using System.Linq;
using DeviceDbModel;
using DeviceDbModel.Models;

namespace CommonVending.DbProvider
{
  public static class StatusDbProvider
  {
    static StatusDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
    
    public static DevStatus GetDeviceLastStatus(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          //var record = context.DeviceLastStatus.Where(w => (w.DeviceId == devId) && (w.MessageDate == context.DeviceLastStatus.Where(m => m.DeviceId == devId).Max(m => m.MessageDate))).ToList();
          var record = context.DeviceLastStatus.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static void WriteDeviceLastStatus(DevStatus record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (record.Id != 0) { context.DeviceLastStatus.Update(record); }
          else { context.DeviceLastStatus.Add(record); }

          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
    
    public static void WriteDeviceErrorStatus(DevErrorStatus record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (record.Id != 0) { context.DeviceErrorStatus.Update(record); }
          else { context.DeviceErrorStatus.Add(record); }

          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
