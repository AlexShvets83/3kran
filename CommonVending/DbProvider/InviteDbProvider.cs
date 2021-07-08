using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;

namespace CommonVending.DbProvider
{
  public static class InviteDbProvider
  {
    static InviteDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }
    
    public static bool CheckInviteCode(string code, string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var count = context.InviteRegistrations.Count(w => (w.Id == code) && (w.Email == email) && (w.ExpirationDate > DateTime.Now));
          //var count = context.InviteRegistrations.Count(w => (w.OwnerId == code) && (w.Email == email) && (w.ExpirationDate > DateTime.Now));
          return count > 0;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    public static string AddInvite(InviteRegistration invite)
    {
      try
      {
        invite.Id = Guid.NewGuid().ToString();
        invite.ExpirationDate = DateTime.Now.AddDays(1);
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          context.InviteRegistrations.Add(invite);
          context.SaveChanges();
          return invite.Id;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static void RemoveInvite(string id)
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

    public static void RemoveAllOldInvitations()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var oldInvitations = context.InviteRegistrations.Where(w => w.ExpirationDate < DateTime.Now);
          context.InviteRegistrations.RemoveRange(oldInvitations);
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static InviteRegistration GetInvite(string id)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var list = context.InviteRegistrations.Where(w => (w.Id == id) && (w.ExpirationDate > DateTime.Now))
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

    public static bool CheckInvite(string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var count = context.InviteRegistrations.Count(w => (w.Email == email) && (w.ExpirationDate > DateTime.Now));
          return count > 0;
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
