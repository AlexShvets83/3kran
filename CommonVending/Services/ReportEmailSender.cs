using System;
using System.Text;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CommonVending.Services
{
  public static class ReportEmailSender
  {
    public static async Task SendNoLink(this IEmailSender emailSender, Device device)
    {
      try
      {
        var ownerID = device.OwnerId;
        if (string.IsNullOrEmpty(ownerID)) return;

        var users = UserDbProvider.GetUserDescendants(ownerID);
        var emailList = string.Empty;

        foreach (var user in users)
        {
          // проверяем нужна ли рассылка, если нужна, тогда брем еще и добавочные адреса

          if ((user.UserAlerts & 1) != 1) continue;

          if (!string.IsNullOrEmpty(emailList)) emailList += ",";
          emailList += user.Email;
          if (!string.IsNullOrEmpty(user.InfoEmails)) emailList += $",{user.InfoEmails}";
        }

        if (string.IsNullOrEmpty(emailList)) return;

        var message = new StringBuilder();
        message.AppendLine("<div>Уважаемый пользователь системы мониторинга «Третий кран»!</div><br />");
        message.AppendLine($"<div>C автоматом {device.Imei} по адресу {device.Address} НЕТ СВЯЗИ!!!.</div><br />");
        message.AppendLine("<div>Чтобы войти в систему мониторинга нажмите на ссылку ниже:</div><br />");
        message.AppendLine($"<div>https://{MainSettings.Settings.MainHost}/</div><br />");
        message.AppendLine("<div>С уважением,</div><br />");
        message.AppendLine("<div><b><i>ООО «Торговый дом «Третий кран».</i></b></div><br />");
        await emailSender.SendEmailAsync(emailList, "Нет связи", message.ToString());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static async Task SendTankEmpty(this IEmailSender emailSender, Device device)
    {
      try
      {
        var ownerID = device.OwnerId;
        if (string.IsNullOrEmpty(ownerID)) return;

        var users = UserDbProvider.GetUserDescendants(ownerID);
        var emailList = string.Empty;

        //var owner = users.Find(f => f.Id == device.OwnerId);
        //if (owner == null) return;

        foreach (var user in users)
        {
          // проверяем нужна ли рассылка, если нужна, тогда брем еще и добавочные адреса

          if (((user.UserAlerts >> 1) & 1) != 1) continue;

          if (!string.IsNullOrEmpty(emailList)) emailList += ",";
          emailList += user.Email;
          if (!string.IsNullOrEmpty(user.InfoEmails)) emailList += $",{user.InfoEmails}";

          //var message = new StringBuilder();
          //message.AppendLine($"<div>Уважаемый {user.FirstName} {user.Patronymic}!</div><br />");
          //message.AppendLine($"<div>У автомата {device.Imei} по адресу {device.Address} БАК ПУСТ!!!.</div><br />");
          //message.AppendLine("<div>Чтобы войти в систему мониторинга нажмите на ссылку ниже:</div><br />");
          //message.AppendLine($"<div></div>https://{MainSettings.Settings.MainHost}/<br />");
          //await emailSender.SendEmailAsync(emailList, "Пустой бак", message.ToString());
        }

        if (string.IsNullOrEmpty(emailList)) return;

        var message = new StringBuilder();
        message.AppendLine("<div>Уважаемый пользователь системы мониторинга «Третий кран»!</div><br />");
        message.AppendLine($"<div>У автомата {device.Imei} по адресу {device.Address} БАК ПУСТ!!!.</div><br />");
        message.AppendLine("<div>Чтобы войти в систему мониторинга нажмите на ссылку ниже:</div><br />");
        message.AppendLine($"<div>https://{MainSettings.Settings.MainHost}/</div><br />");
        message.AppendLine("<div>С уважением,</div><br />");
        message.AppendLine("<div><b><i>ООО «Торговый дом «Третий кран».</i></b></div><br />");
        await emailSender.SendEmailAsync(emailList, "Пустой бак", message.ToString());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static async Task SendNoSales(this IEmailSender emailSender, Device device)
    {
      try
      {
        var ownerID = device.OwnerId;
        if (string.IsNullOrEmpty(ownerID)) return;

        var users = UserDbProvider.GetUserDescendants(ownerID);
        var emailList = string.Empty;

        foreach (var user in users)
        {
          // проверяем нужна ли рассылка, если нужна, тогда брем еще и добавочные адреса

          if (((user.UserAlerts >> 2) & 1) != 1) continue;

          if (!string.IsNullOrEmpty(emailList)) emailList += ",";
          emailList += user.Email;
          if (!string.IsNullOrEmpty(user.InfoEmails)) emailList += $",{user.InfoEmails}";
        }

        if (string.IsNullOrEmpty(emailList)) return;

        var message = new StringBuilder();
        message.AppendLine("<div>Уважаемый пользователь системы мониторинга «Третий кран»!</div><br />");
        message.AppendLine($"<div>У автомата {device.Imei} по адресу {device.Address} НЕТ ПРОДАЖ!!!.</div><br />");
        message.AppendLine("<div>Чтобы войти в систему мониторинга нажмите на ссылку ниже:</div><br />");
        message.AppendLine($"<div>https://{MainSettings.Settings.MainHost}/</div><br />");
        message.AppendLine("<div>С уважением,</div><br />");
        message.AppendLine("<div><b><i>ООО «Торговый дом «Третий кран».</i></b></div><br />");
        await emailSender.SendEmailAsync(emailList, "Нет продаж", message.ToString());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }

    public static async Task SendReport(this IEmailSender emailSender, Device device)
    {
      try
      {
        var ownerID = device.OwnerId;
        if (string.IsNullOrEmpty(ownerID)) return;

        var users = UserDbProvider.GetUserDescendants(ownerID);
        var emailList = string.Empty;

        //var owner = users.Find(f => f.Id == device.OwnerId);
        //if (owner == null) return;

        foreach (var user in users)
        {
          // проверяем нужна ли рассылка, если нужна, тогда брем еще и добавочные адреса

          if ((((user.UserAlerts >> 3) & 1) != 1) || !user.CommerceVisible) continue;

          if (!string.IsNullOrEmpty(emailList)) emailList += ",";
          emailList += user.Email;
          if (!string.IsNullOrEmpty(user.InfoEmails)) emailList += $",{user.InfoEmails}";
        }

        if (string.IsNullOrEmpty(emailList)) return;

        var lastState = StatusDbProvider.GetDeviceLastStatus(device.Id);
        var data = $"<div>У автомата {device.Imei} по адресу {device.Address} Нет данных!!!.</div><br />";
        if (lastState != null)
        {
          var se = string.Empty;
          switch (device.Currency)
          {
            case "RUB":
              se = "₽";
              break;
            case "KZT":
              se = "₸";
              break;
            case "AZN":
              se = "₼";
              break;
            case "UZS":
              se = "сўм";
              break;
            case "BYR":
              se = "Br";
              break;
          }

          data = $"<div>У автомата {device.Imei} по адресу {device.Address} продано {lastState.TotalSold} литров воды на сумму {lastState.TotalMoney}{se}.</div><br />";
        }

        var message = new StringBuilder();
        message.AppendLine("<div>Уважаемый пользователь системы мониторинга «Третий кран»!</div><br />");
        message.AppendLine("<div>Ознакомьтесь с ежедневным отчетом о состоянии ваших автоматов:</div><br />");
        message.AppendLine(data);
        message.AppendLine("<div>Чтобы войти в систему мониторинга нажмите на ссылку ниже:</div><br />");
        message.AppendLine($"<div>https://{MainSettings.Settings.MainHost}/</div><br />");
        message.AppendLine("<div>С уважением,</div><br />");
        message.AppendLine("<div><b><i>ООО «Торговый дом «Третий кран».</i></b></div><br />");
        await emailSender.SendEmailAsync(emailList, "Ежедневный отчет о продажах от системы мониторинга «Третий кран»", message.ToString());
      }
      catch (Exception ex) { Console.WriteLine(ex); }
    }
  }
}
