using System;

namespace DeviceDbModel.Models
{
  public class DevErrorStatus
  {
    public long Id { get; set; }

    public string DeviceId { get; set; }
    
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
    public float? Temperature { get; set; }

    /// <summary>
    /// 0 - ок, 1 - бак пуст
    /// </summary>
    public int Status { get; set; }

    public Device Device { get; set; }
  }
}
