namespace ThirdVendingWebApi.Models.Users
{
  public class UserOwnerSetModel
  {
    /// <summary>
    ///   Ид пользователя из базы данных
    /// </summary>
    public string Id { get; set; }

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
    ///   Email, электронная почта
    /// </summary>
    public string Email { get; set; }

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
