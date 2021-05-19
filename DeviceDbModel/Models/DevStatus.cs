using System;

namespace DeviceDbModel.Models
{
  public class DevStatus
  {
    //timestamp":1536657811,"totalSold":19.540,"totalMoney":1306.790,"status":0
    public long Id { get; set; }

    public string DeviceId { get; set; }

    //public string Imei { get; set; }

    public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    public double Timestamp { get; set; }

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

    public Device Device { get; set; }
  }
}
