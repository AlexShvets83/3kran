using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ThirdVendingWebApi.Models.Users
{
  /// <summary>
  /// </summary>
  public class UserAccountAdminEdit
  {
    /// <summary>
    ///   Ид пользователя из базы данных
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///   Логин или имя пользователя
    /// </summary>
    [JsonProperty("login")]
    [JsonPropertyName("login")]
    public string UserName { get; set; }

    /// <summary>
    ///   Имя
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///   Фамилия
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///   Email, электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///   Ссылка на изображение
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    ///   Активирован ли пользователь
    /// </summary>
    public bool Activated { get; set; }

    /// <summary>
    ///   Язык
    /// </summary>
    public string LangKey { get; set; }

    /// <summary>
    ///   Кем создан
    /// </summary>
    public string CreatedBy { get; set; }

    /// <summary>
    ///   Когда создан
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    ///   Кем последний раз изменен
    /// </summary>
    public string LastModifiedBy { get; set; }

    /// <summary>
    ///   Когда последний раз изменен
    /// </summary>
    public DateTime LastModifiedDate { get; set; }

    /// <summary>
    /// </summary>
    public string[] Authorities { get; set; }

    /// <summary>
    ///   Организация
    /// </summary>
    public string Organization { get; set; }

    /// <summary>
    /// </summary>
    [JsonProperty("phone")]
    [JsonPropertyName("phone")]
    public string PhoneNumber { get; set; }

    /// <summary>
    ///   Город
    /// </summary>
    public string City { get; set; }

    /// <summary>
    ///   Массив имейлов для рассылки писем с авариями и событиями
    /// </summary>
    public string[] InfoEmails { get; set; }

    /// <summary>
    ///   Массив аварий и событий
    /// </summary>
    public AlertTypes[] Alerts { get; set; }

    /// <summary>
    ///   Отчество
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string OwnerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool CommerceVisible { get; set; }
  }
}
