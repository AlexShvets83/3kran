using System;

namespace CommonVending.Model
{
  public class DevStatusView
  {
    public DateTime MessageDate { get; set; }

    /// <summary>
    /// Объем проданной воды
    /// </summary>
    public float TotalSold { get; set; }

    /// <summary>
    /// Сумма принятых наличных
    /// </summary>
    public float TotalMoney { get; set; }

    /// <summary>
    /// Текущая температура
    /// </summary>
    public float Temperature { get; set; }

    /// <summary>
    /// 0 - ок, 1 - бак пуст
    /// </summary>
    public int Status { get; set; }
  }
}
