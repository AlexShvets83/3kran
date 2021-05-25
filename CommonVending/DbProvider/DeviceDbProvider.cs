using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonVending.DbProvider
{
  public static class DeviceDbProvider
  {
    static DeviceDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
    
    public static bool CheckInviteCode(string code, string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var count = context.InviteRegistrations.Count(w => (w.UserId == code) && (w.Email == email) && (w.ExpirationDate > DateTime.Now));
          return count > 0;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    public static InviteRegistration GetInvite(string ownerID, string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.InviteRegistrations
            .Where(w => (w.UserId == ownerID) && (w.Email == email) && (w.ExpirationDate > DateTime.Now))
            .OrderByDescending(o => o.ExpirationDate)
            .ToList();
          return list.Count > 0 ? list[0] : null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static void RemoveIvite(string id)
    {
      try
      {
        var invite = new InviteRegistration {Id = id};
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.InviteRegistrations.Remove(invite);
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static List<Country> GetCountries()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.Countries.OrderBy(o => o.Id).ToList();
          return list;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<Device> GetDevices()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.Devices.OrderBy(o => o.Id).ToList();
          return list;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static Device GetDeviceByImei(string emei)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.Devices.Where(w => w.Imei == emei).ToList();
          return list.Count == 1 ? list[0] : null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static Device GetDeviceById(string id)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.Devices.Where(w => w.Id == id).ToList();
          return list.Count == 1 ? list[0] : null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static async Task<bool> AddDevice(Device dev)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          await context.Devices.AddAsync(dev);
          await context.SaveChangesAsync();

          return true;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    public static void RemoveDevice(Device device)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (device != null)
          {
            context.Entry(device).State = EntityState.Deleted;
            context.SaveChanges();
          }
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static void RemoveDeviceById(string id)
    {
      var device = GetDeviceById(id);
      RemoveDevice(device);

      //try
      //{
      //  var device = GetDeviceById(id);
      //  using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
      //  {
      //    if (device != null)
      //    {
      //      context.Entry(device).State = EntityState.Deleted;
      //      context.SaveChanges();
      //    }
      //  }
      //}
      //catch (Exception ex) { Console.WriteLine(ex); }
    }

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
    
    public static DevSale GetDeviceLastSale(string devId)
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

    public static DevEncash GetDeviceLastEncash(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceEncashes.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
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

    public static DevInfo GetDevInfo(string devId, string name)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceInfos.Where(w => (w.DeviceId == devId) && (w.Name == name)).OrderByDescending(o => o.MessageDate).Take(1).ToList();
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

    public static DevSetting GetLastSettings(string devId, string topic)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceSettings.Where(w => (w.DeviceId == devId) && (w.Topic == topic)).OrderByDescending(o => o.MessageDate).Take(1).ToList();
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

    public static void WriteDeviceLastInfo(DevInfo record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (record.Id != 0) { context.DeviceInfos.Update(record); }
          else { context.DeviceInfos.Add(record); }

          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static void WriteDeviceSettings(DevSetting record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (record.Id != 0) { context.DeviceSettings.Update(record); }
          else { context.DeviceSettings.Add(record); }

          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
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

    public static void InsertDeviceEncash(DevEncash record)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.DeviceEncashes.Add(record);
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

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
  }
}
