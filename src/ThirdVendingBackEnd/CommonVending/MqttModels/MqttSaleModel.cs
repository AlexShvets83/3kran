using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CommonVending.MqttModels
{
  public class MqttSaleModel
  {
    public double Timestamp { get; set; }

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
    public MqttMoney[] Coins { get; set; }

    /// <summary>
    ///   Количество и номинал принятых купюр [{"value":50.000,"amount":1}]
    /// </summary>
    public MqttMoney[] Bills { get; set; }

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
  }

  public class MqttMoney
  {
    /*
Количество и номинал принятых купюр
Пример:
"coins":[{"value":2.000,"amount":1}] 
value – float
amount – int
     */
    [JsonProperty("value")]
    [JsonPropertyName("value")]
    public float Value { get; set; }

    [JsonProperty("amount")]
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
  }
}
