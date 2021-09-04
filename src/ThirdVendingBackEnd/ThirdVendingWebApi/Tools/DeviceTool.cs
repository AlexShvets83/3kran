using DeviceDbModel;
using DeviceDbModel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    internal static bool WriteCommand(string cmd, out string result)
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

    //  internal async Task SendEmail_TaknEmpy()
    //  {
    //    try
    //    {
    //      var addressees = new StringBuilder();
    //      for (var i = 0; i < model.Addressees.Count; i++)
    //      {
    //        addressees.Append(model.Addressees[i]);
    //        if (i != model.Addressees.Count - 1) addressees.Append(',');
    //      }

    //      await _emailSender.SendEmailAsync(addressees.ToString(), model.EmailTheme, model.EmailBody);

    //      //await UserDbProvider.AddLog(new LogUsr
    //      //{
    //      //  UserId = user.Id,
    //      //  Email = user.Email,
    //      //  Phone = user.PhoneNumber,
    //      //  LogDate = DateTime.Now,
    //      //  Message = $"Пользователь {user} отослал приглашение [{invite.Email}] на роль [{invite.Role}]"
    //      //});
    //    }
    //    catch (Exception ex)
    //    {
    //      Console.WriteLine(ex);
    //    }
    //  }
  }
}
