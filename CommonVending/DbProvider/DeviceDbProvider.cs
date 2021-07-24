using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.Model;
using CommonVending.MqttModels;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CommonVending.DbProvider
{
  public static class DeviceDbProvider
  {
    static DeviceDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
    
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

    public static List<DevView> GetAllDevices(List<string> users = null, bool isCommerce = true)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var query = from device in context.Set<Device>()
                      join person in context.Set<ApplicationUser>() on device.OwnerId equals person.Id into grouping
                      //join p in context.Set<ApplicationUser>()
                      //  on b.BlogId equals p.BlogId into grouping
                      from person in grouping.DefaultIfEmpty()
                      orderby device.Id
                      select new DevView
                      {
                        Id = device.Id, Imei = device.Imei, OwnerId = device.OwnerId, Address = device.Address,
                        TimeZone = device.TimeZone, Currency = device.Currency, Phone = device.Phone, OwnerName = person.LastName,
                        OwnerEmail = person.Email
                      };
          if (users != null)
          {
            query = from device in context.Set<Device>()
                    join person in context.Set<ApplicationUser>() on device.OwnerId equals person.Id into grouping
                    //join p in context.Set<ApplicationUser>()
                    //  on b.BlogId equals p.BlogId into grouping
                    from person in grouping.DefaultIfEmpty()
                    where users.Contains(device.OwnerId)
                    orderby device.Id
                    select new DevView
                    {
                      Id = device.Id, Imei = device.Imei, OwnerId = device.OwnerId, Address = device.Address,
                      TimeZone = device.TimeZone, Currency = device.Currency, Phone = device.Phone, OwnerName = person.LastName,
                      OwnerEmail = person.Email
                    };
          }

          var devList = query.AsEnumerable().ToList();
          foreach (var dev in devList)
          {
            var currDate = DateTime.UtcNow.AddHours(dev.TimeZone);
            var alerts = new List<string>();
            var lastStatus = context.DeviceLastStatus.Where(w => w.DeviceId == dev.Id).OrderByDescending(o => o.MessageDate).Take(1).ToList();
            dev.LastStatus = lastStatus.Count > 0 ? lastStatus[0].GetNewObj<DevStatusView>() : null;
            if ((dev.LastStatus == null) || (dev.LastStatus.MessageDate.AddMinutes(32) < currDate)) { alerts.Add("NO_LINK"); }

            if ((dev.LastStatus != null) && (dev.LastStatus.Status == 1)) { alerts.Add("TANK_EMPTY"); }

            var lastSale = context.DeviceSales.Where(w => w.DeviceId == dev.Id).OrderByDescending(o => o.MessageDate).Take(1).ToList();
            dev.LastSale = (lastSale.Count > 0) && isCommerce ? lastSale[0].GetNewObj<DevSaleView>() : null;
            if ((dev.LastSale == null) || (dev.LastSale.MessageDate.AddHours(2) < currDate)) { alerts.Add("NO_SALES"); }

            dev.Alerts = alerts.Count > 0 ? alerts.ToArray() : null;

            var clList = context.DeviceSettings.Where(w => (w.DeviceId == dev.Id) && (w.TopicType == 1)).OrderByDescending(o => o.MessageDate).Take(1).ToList();
            var lastCln = clList.Count > 0 ? JsonConvert.DeserializeObject<CleanerStatusView>(clList[0].Payload) : null;
            if (lastCln != null) lastCln.MessageDate = clList[0].MessageDate;
            dev.LastCleanerStatus = lastCln;
          }

          return devList;
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

    public static async Task<bool> AddOrEditDevice(Device dev)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          if (!string.IsNullOrEmpty(dev.Id)) { context.Devices.Update(dev); }
          else
          {
            dev.Id = Guid.NewGuid().ToString();
            await context.Devices.AddAsync(dev);
          }

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

    public static async Task SetDeviceOwner(string deviceId, string ownerId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var updDevice = new Device
          {
            Id = deviceId, OwnerId = ownerId
          };
          updDevice.OwnerId = ownerId;
          context.Devices.Attach(updDevice).Property(x => x.OwnerId).IsModified = true;
          await context.SaveChangesAsync();

          //var list = context.Devices.Where(w => w.Id == deviceId).ToList();
          //return list.Count == 1 ? list[0] : null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        //return null;
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

    public static DevSetting GetLastSettings(string devId, int topicType)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceSettings.Where(w => (w.DeviceId == devId) && (w.TopicType == topicType)).OrderByDescending(o => o.MessageDate).Take(1).ToList();
          return record.Count == 0 ? null : record[0];
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
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

    public static List<DevEncash> GetDeviceEncash(string devId, int limit)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceEncashes.Where(w => w.DeviceId == devId).OrderByDescending(o => o.MessageDate).Take(limit).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<DevEncash> GetDeviceEncash(string devId, DateTime from, DateTime to)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceEncashes.Where(w => (w.DeviceId == devId) && (w.MessageDate >= from) && (w.MessageDate <= to)).OrderByDescending(o => o.MessageDate).ToList();
          return record;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<DevInfo> GetDeviceInfos(string devId)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var record = context.DeviceInfos.Where(w => (w.DeviceId == devId)).OrderByDescending(o => o.Name).ToList();
          return record;
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
