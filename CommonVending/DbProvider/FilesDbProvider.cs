using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonVending.DbProvider
{
  public static class FilesDbProvider
  {
    static FilesDbProvider() { DeviceDBContextFactory = new DeviceDBContextFactory(); }

    public static DeviceDBContextFactory DeviceDBContextFactory { get; }

    public static List<FileModel> GetFiles()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var files = context.Files.OrderBy(o => o.Id).ToList();
          return files;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static List<FileModel> GetVisibleFiles()
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var files = context.Files.Where(w => w.Visible).OrderBy(o => o.Id).ToList();
          return files;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }

    public static FileModel GetFile(string id)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var files = context.Files.Where(w => w.Id == id).ToList();
          return files.Count == 1 ? files[0] : null;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return null;
      }
    }
    
    public static async Task<bool> AddFile(FileModel file)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          await context.Files.AddAsync(file);
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
    
    public static void RemoveFile(string id)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var file = new FileModel {Id = id};
          context.Entry(file).State = EntityState.Deleted;
          context.SaveChanges();
        }
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
    
    public static async Task SetFileDescription(string fileId, string dsc)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var updFile = new FileModel
          {
            Id = fileId, Description = dsc
          };
          context.Files.Attach(updFile).Property(x => x.Description).IsModified = true;
          await context.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    public static async Task SetFileVisible(string fileId, bool visible)
    {
      try
      {
        using (var context = DeviceDBContextFactory.CreateDbContext(new string[1]))
        {
          var updFile = new FileModel
          {
            Id = fileId, Visible = visible
          };
          context.Files.Attach(updFile).Property(x => x.Visible).IsModified = true;
          await context.SaveChangesAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }
  }
}
