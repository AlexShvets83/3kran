namespace CommonVending.MqttModels
{
  public class MqttDeviceSettings
  {
    /*{
    "timestamp":1619685099,
    "pulsesPerLitre":450,
    "currencySign":0,
    "pricePerLitre":3.000,
    "priceCard":2.000,
    "":131.000,
    "":0.000,
    "pulseValueCoin":1.000,
    "pulseValueBill":50.000,
    "logo":2,
    "maintain":0,
    "date":1610582400,
    "phone":"89638700159",
    "therm":10.000,
    "language":1,
    "display":0}
     */

    /// <summary>
    /// Дата формирования сообщения
    /// </summary>
    public double Timestamp { get; set; }
    
    /// <summary>
    /// Количество импульсов счетчика воды на литр
    /// </summary>
    public int PulsesPerLitre { get; set; }

    /// <summary>
    /// Цена за литр
    /// </summary>
    public float PricePerLitre { get; set; }

    /// <summary>
    /// Цена за литр за безнал. расчет
    /// </summary>
    public float PriceCard { get; set; }

    /// <summary>
    /// Цена импульса монетоприемника !!!!!!!! был int
    /// </summary>
    public float PulseValueCoin { get; set; }

    /// <summary>
    /// Цена импульса купюроприёмника !!!!!!!! был int
    /// </summary>
    public float PulseValueBill { get; set; }

    /// <summary>
    /// Телефон техподдержки
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Дата смены фильтров
    /// </summary>
    public double Date { get; set; }

    /// <summary>
    /// Заданная температура 
    /// </summary>
    public float Therm { get; set; }

    /// <summary>
    /// Приглашение, отображаемое на экране 0-2
    /// </summary>
    public int Logo { get; set; }

    /// <summary>
    /// Режим обслуживания включен 0-1
    /// </summary>
    public int Maintain { get; set; }

    /// <summary>
    /// Язык интерфейса 0-2
    /// </summary>
    public int Language { get; set; }

    /// <summary>
    /// Тип дисплея 0-1
    /// </summary>
    public int Display { get; set; }

    /// <summary>
    /// Валюта 0-4
    /// </summary>
    public int CurrencySign { get; set; }
  }
}
