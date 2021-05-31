using System;

namespace CommonVending.Model
{
  public class CleanerStatusView
  {
    /// <summary>
    ///   Дата формирования сообщения
    /// </summary>
    public DateTime MessageDate { get; set; }

    /// <summary>
    ///   Значение с датчика TDS, (total dissolved solids) означает массу твердого остатка,
    ///   которая получится, если всю воду испарить.
    /// </summary>
    public float Tds { get; set; }

    /// <summary>
    ///   Напряжение питания контроллера
    /// </summary>
    public float Voltage { get; set; }

    /// <summary>
    ///   Счетчик входящей воды
    /// </summary>
    public float VolumeIn { get; set; }

    /// <summary>
    ///   Счетчик пермеата
    /// </summary>
    public float VolumePerm { get; set; }

    /// <summary>
    ///   Счетчик концентрата
    /// </summary>
    public float VolumeConc { get; set; }

    /// <summary>
    ///   Отношение пермеат/концентрат
    /// </summary>
    public float Pcr { get; set; }

    /// <summary>
    ///   0 – ожидание (Полный бак)
    ///   1 – фильтрация (Фильтрация)
    ///   2 – промывка (Промывка)
    ///   3 – нет давления на входе (Нет воды)
    ///   4 – затопление (Затопление)
    ///   5 – настройка (Настройка)
    /// </summary>
    public int Status { get; set; }

    public string StrStatus
    {
      get
      {
        switch (Status)
        {
          case 0: return "Полный бак";
          case 1: return "Фильтрация";
          case 2: return "Промывка";
          case 3: return "Нет воды";
          case 4: return "Затопление";
          case 5: return "Настройка";
          default: return "Неизвестно";
        }
      }
    }
  }
}
