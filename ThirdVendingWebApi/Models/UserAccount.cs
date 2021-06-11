using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace ThirdVendingWebApi.Models
{
  /// <summary>
  ///   Учетная запись пользователя
  /// </summary>
  public class UserAccount
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
    public bool? Activated { get; set; }

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
    ///   From DB
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [XmlIgnore]
    public string InfoEmails { get; set; }

    /// <summary>
    ///   Массив имейлов для рассылки писем с авариями и событиями
    /// </summary>
    [JsonProperty("infoEmails")]
    [JsonPropertyName("infoEmails")]
    public string[] InfoEmailsArray
    {
      get
      {
        if (string.IsNullOrEmpty(InfoEmails)) return Array.Empty<string>();

        var arr = InfoEmails.Split(",");
        return arr.Length > 0 ? arr : new[] {InfoEmails};
      }
    }

    /// <summary>
    ///   From DB
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public int UserAlerts { get; set; }

    /// <summary>
    ///   Массив аварий и событий
    /// </summary>
    public List<AlertTypes> Alerts => new()
    {
      new AlertTypes {Type = "NO_LINK", Active = (UserAlerts & 1) == 1, Description = "Нет данных от автомата более 10 минут"},
      new AlertTypes {Type = "TANK_EMPTY", Active = ((UserAlerts >> 1) & 1) == 1, Description = "Бак пуст"},
      new AlertTypes {Type = "NO_SALES", Active = ((UserAlerts >> 2) & 1) == 1, Description = "Нет продаж более трех часов"},
      new AlertTypes {Type = "REPORT", Active = ((UserAlerts >> 3) & 1) == 1, Description = "Ежедневный отчет"}
    };

    /// <summary>
    ///   Отчество
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    ///   Роль
    /// </summary>
    public string Role { get; set; }
  }
}
