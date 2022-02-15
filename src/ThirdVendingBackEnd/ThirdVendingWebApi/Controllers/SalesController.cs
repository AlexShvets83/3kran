using CommonVending.DbProvider;
using CommonVending.MqttModels;
using DeviceDbModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceDbModel;
using Microsoft.AspNetCore.Identity;
using ThirdVendingWebApi.Models;
using ThirdVendingWebApi.Models.Device;

namespace ThirdVendingWebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SalesController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public SalesController(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet("info")]
    [Authorize]
    public async Task<IActionResult> Get(string deviceId, DateTime? from, DateTime? to)
    {
      var user = await _userManager.GetUserAsync(HttpContext.User);
      if (user == null) return NotFound("Пользователь не найден!");
      if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

      if ((user.Role == Roles.Technician) && !user.CommerceVisible) return new ObjectResult(null);
      //todo check role and permitions
      return await Task.Factory.StartNew(() =>
      {
        var sales = (from != null) && (to != null) ? SalesDbProvider.GetDeviceSales(deviceId, from.Value, to.Value) : SalesDbProvider.GetDeviceSales(deviceId, 5);
        var retListSales = new List<DeviceSaleModel>();
        foreach (var sale in sales)
        {
          var sl = new DeviceSaleModel
          {
            MessageDate = sale.MessageDate,
            Quantity = sale.Quantity,
            PaymentType = sale.PaymentType,
            AmountCard = sale.PaymentType == 1 ? sale.Amount : null,
            AmountCash = (sale.PaymentType == 0) || (sale.PaymentType == -1) ? sale.Amount : null,
            Coins = sale.Coins != null ? JsonConvert.DeserializeObject<MqttMoney[]>(sale.Coins) : null,
            Bills = sale.Bills != null ? JsonConvert.DeserializeObject<MqttMoney[]>(sale.Bills) : null,
            RfidCard = sale.RfidCard, CoinsChange = sale.CoinsChange, Rest = sale.Rest
          };
          sl.AmountCoin = SummMoney(sl.Coins);
          sl.AmountBill = SummMoney(sl.Bills);

          retListSales.Add(sl);
        }
        //var retList = sales.Select(Main.GetNewObj<DevSaleView>).ToList();
        return new ObjectResult(retListSales);
      });
    }

    internal static float? SummMoney(MqttMoney[] saleMonies)
    {
      if (!(saleMonies?.Length > 0)) return null;

      var retSumm = saleMonies.Sum(sm => (sm.Amount * sm.Value));

      return retSumm > 0.0 ? retSumm : null;
    }

    [HttpGet("/api/sales-graph/{id}")]
    public async Task<IActionResult> GetChartData(string id, string period)
    {
      try
      {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return NotFound("Пользователь не найден!");
        if (!user.Activated.GetValueOrDefault()) return StatusCode(403, "Пользователь деактивирован!");

        if ((user.Role == Roles.Technician) && !user.CommerceVisible) return new ObjectResult(null);
        //todo check role and permitions
        var device = DeviceDbProvider.GetDeviceById(id);
        if (device == null) return NotFound("Автомат не найден!");

        var timeNow = DateTime.UtcNow.AddHours(device.TimeZone);
        DateTime from;
        List<DevSale> sales;
        var retData = new List<SaleChartModel>();

        switch (period)
        {
          case "DAYS":
            from = timeNow.AddDays(-1);
            sales = SalesDbProvider.GetDeviceSales(id, from, timeNow);

            if ((sales == null) || (sales.Count == 0)) return new ObjectResult(null);

            while (from <= timeNow)
            {
              retData.Add(new SaleChartModel {Date = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0), FormattedDate = from.ToString("HH")});
              from = from.AddHours(1);
            }

            foreach (var sale in sales)
            {
              var rdM = retData.Find(f => (f.Date.Day == sale.MessageDate.Day) && (f.Date.Hour == sale.MessageDate.Hour));
              if (rdM != null)
              {
                rdM.Amount += sale.Amount;
                rdM.Quantity += sale.Quantity;
              }
            }

            return new ObjectResult(retData);
          case "MONTHS":
            from = timeNow.AddMonths(-1);
            sales = SalesDbProvider.GetDeviceSales(id, from, timeNow);

            if ((sales == null) || (sales.Count == 0)) return new ObjectResult(null);

            while (from <= timeNow)
            {
              retData.Add(new SaleChartModel {Date = new DateTime(from.Year, from.Month, from.Day), FormattedDate = from.ToString("dd.MM")});
              from = from.AddDays(1);
            }

            foreach (var sale in sales)
            {
              var rdM = retData.Find(f => (f.Date.Month == sale.MessageDate.Month) && (f.Date.Day == sale.MessageDate.Day));
              if (rdM != null)
              {
                rdM.Amount += sale.Amount;
                rdM.Quantity += sale.Quantity;
              }
            }

            return new ObjectResult(retData);
          case "YEARS":
            from = timeNow.AddYears(-1);
            sales = SalesDbProvider.GetDeviceSales(id, from, timeNow);

            if ((sales == null) || (sales.Count == 0)) return new ObjectResult(null);

            while (from <= timeNow)
            {
              retData.Add(new SaleChartModel
              {
                Date = new DateTime(from.Year, from.Month, from.Day), FormattedDate = from.ToString("MM.yy")

                //FormattedDate = from.ToString("MMMM", new CultureInfo("ru-RU"))
              });
              from = from.AddMonths(1);
            }

            foreach (var sale in sales)
            {
              var rdM = retData.Find(f => (f.Date.Year == sale.MessageDate.Year) && (f.Date.Month == sale.MessageDate.Month));
              if (rdM != null)
              {
                rdM.Amount += sale.Amount;
                rdM.Quantity += sale.Quantity;
              }
            }

            return new ObjectResult(retData);
          default: return new ObjectResult(null);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        return StatusCode(500, ex);
      }
    }
  }
}
