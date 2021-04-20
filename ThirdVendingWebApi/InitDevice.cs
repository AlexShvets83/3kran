using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonVending.DbProvider;
using DeviceDbModel.Models;

namespace ThirdVendingWebApi
{
  public class InitDevice
  {
    public InitDevice()
    {
      var dev = new Device()
      {
        Id = Guid.NewGuid().ToString(),
        Address = "Москва ываываыва",
        Currency = "RUB",
        DeviceId = "1234567890123456",
        Phone = "123456789",
        TimeZone = 2,
        //OwnerId = ""
      };

      var resalt = DeviceDbProvider.AddDevice(dev).Result;

    }
  }
}
