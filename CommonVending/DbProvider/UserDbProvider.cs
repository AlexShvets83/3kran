using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable SpecifyStringComparison

namespace CommonVending.DbProvider
{
  public static class UserDbProvider
  {
    static UserDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }

    /*
     WITH RECURSIVE t
    AS
    (
        SELECT * 
          FROM "AspNetUsers" sa
         WHERE sa.id = 'a07e4a13-3f93-4a9d-8ac4-4687adbbd83f'
         UNION ALL
        SELECT next.*
          FROM t prev
          JOIN "AspNetUsers" next ON (next.owner_id = prev.id)
    )
    SELECT * FROM t;
     */

    public static List<ApplicationUser> GetUserDescendants(string id, bool isInRoleOwner = false)
    {
      try
      {
        /*
         select t1.*, t2.email "OwnerEmail" from (WITH RECURSIVE t AS
          (SELECT * FROM "AspNetUsers" sa
                WHERE sa.id = 'a07e4a13-3f93-4a9d-8ac4-4687adbbd83f'
                UNION ALL
                SELECT next.*
                  FROM t prev
                JOIN "AspNetUsers" next ON (next.owner_id = prev.id)
                  )
                SELECT * FROM t) t1
            JOIN "AspNetUsers" t2 on t1.owner_id=t2.Id
         
         */
        var mainQuery = @"select t1.*, t2.email ""OwnerEmail"" from (";
        var query = $@"
WITH RECURSIVE t AS
  (SELECT * FROM ""AspNetUsers"" sa
        WHERE sa.id = '{
            id
          }'
        UNION ALL
        SELECT next.*
          FROM t prev
        JOIN ""AspNetUsers"" next ON (next.owner_id = prev.id)
          )
        SELECT * FROM t";
        if (isInRoleOwner) { query += " where t.role = 'owner'"; }

        query += " Order by id;";
        mainQuery += query;
        mainQuery += @") t1 JOIN ""AspNetUsers"" t2 on t1.owner_id=t2.Id";

        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var users = context.Users.FromSqlRaw(query).ToList();
          return users;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static async Task<bool> AddLog(LogUsr log)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          await context.UserLog.AddAsync(log);
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

    public static List<LogUsr> GetUserLog()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var log = context.UserLog.ToList();
          return log;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static bool CheckUserByEmail(string id, string email)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var cmdList = context.Users.Where(w => (w.Id != id) && (w.Email.ToUpper() == email.ToUpper())).ToList();
          return cmdList.Count == 0;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }

    public static bool CheckUserByPhone(string id, string phone)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var cmdList = context.Users.Where(w => (w.Id != id) && (w.PhoneNumber.ToUpper() == phone.ToUpper())).ToList();
          return cmdList.Count == 0;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return false;
      }
    }
    
    //    public static List<UserApp> GetAllUsers(string id, bool isInRoleOwner = false)
    //    {
    //      try
    //      {
    //        var mainQuery = @"select t1.*, t2.email ""OwnerEmail"" from (";
    //        var query = $@"
    //WITH RECURSIVE t AS
    //  (SELECT * FROM ""AspNetUsers"" sa
    //        WHERE sa.id = '{id}'
    //        UNION ALL
    //        SELECT next.*
    //          FROM t prev
    //        JOIN ""AspNetUsers"" next ON (next.owner_id = prev.id)
    //          )
    //        SELECT * FROM t";
    //        if (isInRoleOwner)
    //        {
    //          query += " where t.role = 'owner'";
    //        }

    //        query += " Order by id;";
    //        mainQuery += query;
    //        mainQuery += @") t1 JOIN ""AspNetUsers"" t2 on t1.owner_id=t2.Id";

    //        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
    //        {
    //          var users = context.Users.FromSqlRaw(mainQuery).ToList();
    //          return users;
    //        }
    //      }
    //      catch (Exception ex)
    //      {
    //        Console.WriteLine(ex);
    //        return null;
    //      }
    //    }

    //public static List<ApplicationUser> GetApplicationUsers()
    //{
    //  using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
    //  {
    //    //var usersWithRoles = (from user in context.Users
    //    //                      from userRole in user.Role
    //    //                      join role in context.Roles on userRole.RoleId equals 
    //    //                        role.Id
    //    //                      select new UserViewModel()
    //    //                      {
    //    //                        Username = user.UserName,
    //    //                        Email = user.Email,
    //    //                        Role = role.Name
    //    //                      }).ToList();
    //  }

    //}
  }
}
