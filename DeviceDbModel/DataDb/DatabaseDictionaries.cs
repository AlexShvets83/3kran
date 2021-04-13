using DeviceDbModel.Models;
using System.Collections.Generic;

namespace DeviceDbModel.DataDb
{
  public static class DatabaseDictionaries
  {
    public static readonly List<Country> CountriesDic = new()
    {
      new Country {Name = "Россия", Alpha2Code = "RU", Alpha3Code = "RUS", Code = 643},
      new Country {Name = "Казахстан", Alpha2Code = "KZ", Alpha3Code = "KAZ", Code = 398},
      new Country {Name = "Азербайджан", Alpha2Code = "AZ", Alpha3Code = "AZE", Code = 031},
      new Country {Name = "Узбекистан", Alpha2Code = "UZ", Alpha3Code = "UZB", Code = 860},
      new Country {Name = "Белоруссия", Alpha2Code = "BY", Alpha3Code = "BLR", Code = 112}
    };
  }
}
