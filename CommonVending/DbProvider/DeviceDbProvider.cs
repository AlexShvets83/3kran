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

    public static async Task<bool> AddDevice(Device dev)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var result = await context.Devices.AddAsync(dev);
          var asd = await context.SaveChangesAsync();

          return true;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }
  }
}
