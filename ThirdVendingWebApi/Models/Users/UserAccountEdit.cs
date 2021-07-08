namespace ThirdVendingWebApi.Models.Users
{
  public class UserAccountEdit
  {
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
  }
}
