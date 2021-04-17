using DeviceDbModel.Models;
using System.Collections.Generic;

namespace DeviceDbModel.DataDb
{
  public static class DatabaseDictionaries
  {
    public static readonly List<Country> CountriesDic = new()
    {
      new Country {Id = 1, Name = "Россия", Alpha2Code = "RU", Alpha3Code = "RUS", Code = 643},
      new Country {Id = 2, Name = "Казахстан", Alpha2Code = "KZ", Alpha3Code = "KAZ", Code = 398},
      new Country {Id = 3, Name = "Азербайджан", Alpha2Code = "AZ", Alpha3Code = "AZE", Code = 031},
      new Country {Id = 4, Name = "Узбекистан", Alpha2Code = "UZ", Alpha3Code = "UZB", Code = 860},
      new Country {Id = 5, Name = "Белоруссия", Alpha2Code = "BY", Alpha3Code = "BLR", Code = 112}
    };
  }
}
