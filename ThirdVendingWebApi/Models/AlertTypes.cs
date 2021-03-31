namespace ThirdVendingWebApi.Models
{
  /// <summary>
  /// Тип аварий и событий
  /// </summary>
  public class AlertTypes
  {
    /// <summary>
    /// Тип
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Активен ли
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string Description { get; set; }
  }
}
