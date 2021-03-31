using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ThirdVendingWebApi.Services
{
  // This class is used by the application to send Email and SMS
  // when you turn on two-factor authentication in ASP.NET Identity.
  // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
  public class AuthMessageSender : IEmailSender, ISmsSender
  {
    public Task SendEmailAsync(string email, string subject, string message)
    {
      var smtp = new SmtpClientSettings();
      var client = new SmtpClient
      {
        UseDefaultCredentials = false,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        Host = smtp.Host,
        Port = smtp.Port,
        EnableSsl = smtp.EnableSsl,
        Credentials = new NetworkCredential(smtp.UserName, smtp.Password),
      };
      var messageSend = new MailMessage(smtp.UserName, email, subject, message) {IsBodyHtml = true};
      return client.SendMailAsync(messageSend);
    }

    public Task SendSmsAsync(string number, string message)
    {
      // Plug in your SMS service here to send a text message.
      return Task.FromResult(0);
    }
  }
}
