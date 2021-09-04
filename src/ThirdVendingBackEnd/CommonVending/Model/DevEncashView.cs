using System;

namespace CommonVending.Model
{
  public class DevEncashView
  {
    public DateTime MessageDate { get; set; }

    public float AmountCoin { get; set; }

    public float AmountBill { get; set; }

    public float Amount { get; set; }

    /// <summary>
    ///   Количество и номинал принятых монет [{"value":1.000,"amount":1}
    /// </summary>
    public string Coins { get; set; }

    /// <summary>
    ///   Количество и номинал принятых купюр [{"value":50.000,"amount":1}]
    /// </summary>
    public string Bills { get; set; }
  }
}
