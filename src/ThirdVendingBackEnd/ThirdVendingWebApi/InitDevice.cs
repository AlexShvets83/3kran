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
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "1234567890123456",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Питер",
          Currency = "RUB",
          Imei = "984412565144539",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Питер",
          Currency = "RUB",
          Imei = "997324988453220",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
        Id = null,
        Address = "Питер",
        Currency = "RUB",
        Imei = "504216888443440",
        Phone = "123456789",
        TimeZone = 2,
        //OwnerId = ""
      },
        new()
        {
          Id = null,
          Address = "Питер",
          Currency = "RUB",
          Imei = "339357915628677",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "984778473534049",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "107726859245617",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "497004910809617",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "100506464134555",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "536139870000378",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        },
        new()
        {
          Id = null,
          Address = "Москва",
          Currency = "RUB",
          Imei = "535737247591036",
          Phone = "123456789",
          TimeZone = 2,
          //OwnerId = ""
        }



      };
      
      foreach (var dev in devList)
      {
        var result = DeviceDbProvider.AddOrEditDevice(dev).Result;
      }
      //var dev = new Device()
      //{
      //  Id = null,
      //  Address = "Москва",
      //  Currency = "RUB",
      //  Imei = "1234567890123456",
      //  Phone = "123456789",
      //  TimeZone = 2,
      //  //OwnerId = ""
      //};

      //var result = DeviceDbProvider.AddDevice(dev).Result;

      //dev = new Device()
      //{
      //  Id = null,
      //  Address = "Питер",
      //  Currency = "RUB",
      //  Imei = "984412565144539",
      //  Phone = "123456789",
      //  TimeZone = 2,
      //  //OwnerId = ""
      //};
      //result = DeviceDbProvider.AddDevice(dev).Result;
    }
  }
}
