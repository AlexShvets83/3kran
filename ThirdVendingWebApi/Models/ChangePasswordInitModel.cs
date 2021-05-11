using System;

namespace ThirdVendingWebApi.Models
{
  public class ChangePasswordInitModel
  {
    public string Id { get; set; }

    public string Email { get; set; }

    public DateTime ExpiryDateTime { get; set; }
  }
}
