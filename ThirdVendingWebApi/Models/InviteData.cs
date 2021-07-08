namespace ThirdVendingWebApi.Models
{
  public class InviteData
  {
    /// <summary>
    ///   Email, электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///   Email владельца
    /// </summary>
    public string OwnerEmail { get; set; }

    /// <summary>
    /// Идентификатор страны
    /// </summary>
    public int CountryId { get; set; } 
  }
}
