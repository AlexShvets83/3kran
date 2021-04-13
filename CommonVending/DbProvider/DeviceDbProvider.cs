using System;
using System.Linq;
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

    public static InviteRegistration GetInvite(string code, string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.InviteRegistrations
            .Where(w => (w.UserId == code) && (w.Email == email) && (w.ExpirationDate > DateTime.Now))
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
  }
}
