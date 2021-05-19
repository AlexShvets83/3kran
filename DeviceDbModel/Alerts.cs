using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDbModel
{
  public class Alerts
  {
    public List<DevAl> DevAlert = new List<DevAl>
    {
      new DevAl {CodeType = 0, Message = "Нет продаж"},
      new DevAl {CodeType = 3, Message = "Продажи восстановились"},
      new DevAl {CodeType = -1, Message = "Пропала связь с автоматом"},
      new DevAl {CodeType = 1, Message = "Связь восстановилась"},
      new DevAl {CodeType = -2, Message = "Бак пуст"},
      new DevAl {CodeType = 2, Message = "Бак заполняется"},
    };
  }

  public class DevAl
  {
    public int CodeType { get; set; }

    public string Message { get; set; }

    /*
     Нет продаж
    Продажи восстановились

    Пропала связь с автоматом
    Связь восстановилась

    Бак пуст
    Бак заполняется
     
     
     */
  }
}
