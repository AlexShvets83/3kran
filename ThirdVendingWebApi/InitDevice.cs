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
      var devList = new List<Device>
      {
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "1234567890123456",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Питер",
          Currency = "RUB",
          DeviceId = "984412565144539",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Питер",
          Currency = "RUB",
          DeviceId = "997324988453220",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
        Id = Guid.NewGuid().ToString(),
        Address = "Питер",
        Currency = "RUB",
        DeviceId = "504216888443440",
        Phone = "123456789",
        TimeZone = 2,
        //OwnerId = ""
      },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Питер",
          Currency = "RUB",
          DeviceId = "339357915628677",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "984778473534049",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "107726859245617",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "497004910809617",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "100506464134555",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "536139870000378",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = Guid.NewGuid().ToString(),
          Address = "Москва",
          Currency = "RUB",
          DeviceId = "535737247591036",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        }



      };
      
      foreach (var dev in devList)
      {
        var result = DeviceDbProvider.AddDevice(dev).Result;
      }
      //var dev = new Device()
      //{
      //  Id = Guid.NewGuid().ToString(),
      //  Address = "Москва",
      //  Currency = "RUB",
      //  DeviceId = "1234567890123456",
      //  Phone = "123456789",
      //  TimeZone = 2,
      //  //OwnerId = ""
      //};

      //var result = DeviceDbProvider.AddDevice(dev).Result;

      //dev = new Device()
      //{
      //  Id = Guid.NewGuid().ToString(),
      //  Address = "Питер",
      //  Currency = "RUB",
      //  DeviceId = "984412565144539",
      //  Phone = "123456789",
      //  TimeZone = 2,
      //  //OwnerId = ""
      //};
      //result = DeviceDbProvider.AddDevice(dev).Result;
    }
  }
}
