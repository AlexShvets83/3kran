using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity;

namespace ThirdVendingWebApi.Tools
{
  public class DeviceTool
  {
    public static string GetNewDeviceOwner(ApplicationUser user, IList<string> userRoles)
    {
      if (userRoles.Contains(Roles.Technician) || userRoles.Contains(Roles.DealerAdmin)) { return user.OwnerId; }

      return user.Id;
    }

    internal static string WriteCommand(string cmd)
    {
      try
      {
        using (var process = new Process())
        {
          process.StartInfo.FileName = "/bin/bash";
          process.StartInfo.Arguments = $"-c \"{cmd}\"";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.CreateNoWindow = true;
          process.Start();

          var result = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
          return result;
        }
      }
      catch (Exception exception)
      {
        return exception.Message;
      }
    }

    private static bool WriteCommand(string cmd, out string result)
    {
      try
      {
        using (var process = new Process())
        {
          process.StartInfo.FileName = "/bin/bash";
          process.StartInfo.Arguments = $"-c \"{cmd}\"";
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.CreateNoWindow = true;
          process.Start();

          result = process.StandardOutput.ReadToEnd();
          process.WaitForExit();
          return true;
        }
      }
      catch (Exception exception)
      {
        result = exception.Message;
        return false;
      }
    }
  }
}
