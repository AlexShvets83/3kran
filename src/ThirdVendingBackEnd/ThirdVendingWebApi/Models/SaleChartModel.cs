using System;

namespace ThirdVendingWebApi.Models
{
  public class SaleChartModel
  {
    public DateTime Date { get; set; }

    public string FormattedDate { get; set; }

    public float Amount { get; set; }

    public float Quantity { get; set; }
  }
}
