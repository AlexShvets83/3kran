using System.Threading.Tasks;

namespace ThirdVendingWebApi.Services
{
  public interface ISmsSender
  {
    Task SendSmsAsync(string number, string message);
  }
}
