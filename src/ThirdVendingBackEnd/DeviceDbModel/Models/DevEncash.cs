using System;

namespace DeviceDbModel.Models
{
  public class DevEncash
  {
    public long Id { get; set; }

    public string DeviceId { get; set; }

    //public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    //public double Timestamp { get; set; }

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
    
    /// <summary>
    ///   Объем средств пополнения по NFC карте (общее суммарное значение)
    /// </summary>
    public float? NfcCard { get; set; }

    /// <summary>
    ///   Объем выданных средств при сдаче (значение по текущей оплате)
    /// </summary>
    public float? CoinsChange { get; set; }

    /// <summary>
    ///   Объем не выданных средств (общее суммарное значение)
    /// </summary>
    public float? Rest { get; set; }

    public Device Device { get; set; }
  }
}
