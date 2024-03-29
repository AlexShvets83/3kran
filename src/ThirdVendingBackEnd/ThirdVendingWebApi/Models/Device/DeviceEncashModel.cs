﻿using CommonVending.MqttModels;
using System;

namespace ThirdVendingWebApi.Models.Device
{
  /// <summary>
  ///   Модель инкассации
  /// </summary>
  public class DeviceEncashModel
  {
    /// <summary>
    ///   Дата операции
    /// </summary>
    public DateTime MessageDate { get; set; }

    /// <summary>
    ///   Сумма операции
    /// </summary>
    public float Amount { get; set; }
    
    /// <summary>
    /// Общая сумма монет
    /// </summary>
    public float? AmountCoin { get; set; }

    /// <summary>
    ///   Общая сумма купюр
    /// </summary>
    public float? AmountBill { get; set; }

    /// <summary>
    ///   Количество и номинал принятых монет [{"value":1.000,"amount":1}
    /// </summary>
    public MqttMoney[] Coins { get; set; }

    /// <summary>
    ///   Количество и номинал принятых купюр [{"value":50.000,"amount":1}]
    /// </summary>
    public MqttMoney[] Bills { get; set; }
    
    /// <summary>
    ///   Объем средств пополнения по Rfid карте (общее суммарное значение)
    /// </summary>
    public float? RfidCard { get; set; }
    
    /// <summary>
    ///   Объем выданных средств при сдаче (значение по текущей оплате)
    /// </summary>
    public float? CoinsChange { get; set; }

    /// <summary>
    ///   Объем не выданных средств (общее суммарное значение)
    /// </summary>
    public float? Rest { get; set; }
  }
}
