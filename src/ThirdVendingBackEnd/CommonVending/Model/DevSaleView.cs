using System;

namespace CommonVending.Model
{
  public class DevSaleView
  {
    public DateTime MessageDate { get; set; }

    /// <summary>
    /// Тип продажи 0 - налич., 1 - безнал. (-1) - пополнение карты
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
  }
}
