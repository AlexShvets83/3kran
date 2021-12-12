using System;

namespace DeviceDbModel.Models
{
  public class DevSale
  {
    public long Id { get; set; }

    public string DeviceId { get; set; }

    //public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    //public double Timestamp { get; set; }

    /// <summary>
    ///   Тип продажи 0 - налич., 1 - безнал. (-1) - пополнение карты
    /// </summary>
    public int PaymentType { get; set; }

    /// <summary>
    ///   Объем проданной воды
    /// </summary>
    public float Quantity { get; set; }

    /// <summary>
    ///   Цена
    /// </summary>
    public float Price { get; set; }

    /// <summary>
    ///   Сумма операции
    /// </summary>
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
