namespace CommonVending.MqttModels
{
  public class MqttCleanerSettings
  {
    /// <summary>
    ///   Дата формирования сообщения
    /// </summary>
    public double Timestamp { get; set; }

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
    ///   0 – ожидание
    ///   1 – фильтрация
    ///   2 – промывка
    ///   3 – нет давления на входе
    ///   4 – затопление
    ///   5 – настройка
    /// </summary>
    public int Status { get; set; }
  }
}
