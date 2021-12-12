using System;

namespace DeviceDbModel.Models
{
  public class DevStatus
  {
    //timestamp":1536657811,"totalSold":19.540,"totalMoney":1306.790,"status":0
    public long Id { get; set; }

    public string DeviceId { get; set; }

    //public string Imei { get; set; }

    //public DateTime ReceivedDate { get; set; }

    public DateTime MessageDate { get; set; }

    //public double Timestamp { get; set; }

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

    /// <summary>
    /// Общий счетчик выданной сдачи (после инкассации)
    /// </summary>
    public float? TotalChange { get; set; }

    /// <summary>
    /// Общий счетчик остатка (после инкассации)
    /// </summary>
    public float? TotalRest { get; set; }

    /// <summary>
    /// Общий счетчик пополнения по NFC карте (после инкассации)
    /// </summary>
    public float? NfcCard { get; set; }

    /// <summary>
    /// 0 - В автомате не установлен монетоприемник со сдачей 
    /// 1 - Сдача заканчивается, сообщение на дисплее для клиента (пока 5% от порога “Мало монет для выплаты сдачи”)
    /// 2 - Мало монет для выплаты сдачи, сообщение на сервер для владельца (порог настраиваемый владельцем)
    /// 3 - Сдачи достаточно (больше порога  “Мало монет для выплаты сдачи”, настраиваемого владельцем)
    /// </summary>
    public int? ChangeStatus { get; set; }

    /// <summary>
    /// Доступная для выдачи величина сдачи в монеторпиёмнике.
    /// (порог “Мало монет для выплаты сдачи” будет передаваться в настройках) 
    /// </summary>
    public float? CurrentChange { get; set; }

    /// <summary>
    /// 000000 
    /// 0) billAcceptor - состояние купюроприемника
    ///     0  - Все ОК
    ///     1 - Устройство не работает (неисправно)
    /// 1) coinAcceptor - состояние монетоприемника
    ///     0  - Все ОК
    ///     1 - Устройство не работает (неисправно)
    /// 2) bankTerminal  - состояние терминала оплаты 
    ///     0  - Все ОК
    ///     1 - Устройство не работает (неисправно)
    /// 3) power12V - состояние питания 12V
    ///     0  - Все ОК
    ///     1 - неисправные 12V (PULSE платежные устройства)
    /// 4) power24V - состояние питания 24V
    ///     0  - Все ОК
    ///     1 - неисправные 24V  (MDB платежные устройства)
    /// 5) power220V - состояние питания 220V
    ///     0  - Все ОК
    ///     1 - неисправные 220V (питание насоса, озонатор, термокабель)
    /// </summary>
    public int? Alert { get; set; }

    public Device Device { get; set; }
  }
}
