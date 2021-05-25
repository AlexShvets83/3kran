namespace CommonVending.MqttModels
{
  public class MqttEncashModel
  {
    public double Timestamp { get; set; }

    public float AmountCoin { get; set; }

    public float AmountBill { get; set; }

    public float Amount { get; set; }

    /// <summary>
    ///   Количество и номинал принятых монет [{"value":1.000,"amount":1}
    /// </summary>
    public MqttMoney[] Coins { get; set; }

    /// <summary>
    ///   Количество и номинал принятых купюр [{"value":50.000,"amount":1}]
    /// </summary>
    public MqttMoney[] Bills { get; set; }
  }
}
