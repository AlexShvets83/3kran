using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDbModel.Models;

namespace CommonVending.Model
{
  class DevView
  {
    /*
     {"id":"605f489e2ab79c00016fe6e9",
"address":"Касымханова 16",
"timeZone":6,
"currency":"KZT",
"deviceId":"868328059883104",
"ownerId":"5fd060d336d1a0000185f201",

"lastStatus":{"id":"AXiO6Xiz8Wp6J06xlZx5",
"timestamp":1617305368,
"totalMoney":605,
"totalSold":37,
"temperature":null,
"status":0,
"deviceId":"868328059883104"},

"lastCleanerStatus":null,
"settings":null,
"alerts":["NO_SALES"],
"ownerName":"Легенза",
"ownerEmail":"nataljalegenza@mail.ru",
"phone":"+77059895973",
"info":[]}
     */
    public string Id { get; set; }

    public string Imei { get; set; }

    public string OwnerId { get; set; }

    public string Address { get; set; }

    public int TimeZone { get; set; }

    public string Currency { get; set; }

    public string Phone { get; set; }

    public string OwnerName { get; set; }

    public string OwnerEmail { get; set; }

    public DevStatus LastStatus { get; set; }

    public DevSale LastSale { get; set; }

    public DevAlert LastAlert { get; set; }

    //"lastCleanerStatus":null,
    //"settings":null,
    //"alerts":["NO_SALES"],
    //"info":[]
    //public DevEncash

  }
}
