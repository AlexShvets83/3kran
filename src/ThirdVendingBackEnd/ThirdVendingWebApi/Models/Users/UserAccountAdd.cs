using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ThirdVendingWebApi.Models.Users
{
  /// <summary>
  /// 
  /// </summary>
  public class UserAccountAdd
  {
    /// <summary>
    ///   Логин или имя пользователя
    /// </summary>
    [JsonProperty("login")]
    [JsonPropertyName("login")]
    public string UserName { get; set; }

    /// <summary>
    ///   Email, электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///   Телефон
    /// </summary>
    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///   Имя
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///   Фамилия
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///   Отчество
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    ///   Организация
    /// </summary>
    public string Organization { get; set; }

    /// <summary>
    ///   Город
    /// </summary>
    public string City { get; set; }

    /// <summary>
    ///   Ссылка на изображение
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    ///   Язык
    /// </summary>
    public string LangKey { get; set; }

    /// <summary>
    ///   Массив имейлов для рассылки писем с авариями и событиями
    /// </summary>
    public string[] InfoEmails { get; set; }

    /// <summary>
    /// Массив аварий и событий
    /// </summary>
    public AlertTypes[] Alerts { get; set; }
  }
}
