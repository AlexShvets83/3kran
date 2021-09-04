using System.Threading.Tasks;

namespace CommonVending.Services
{
  public interface ISmsSender
  {
    Task SendSmsAsync(string number, string message);
  }
}
