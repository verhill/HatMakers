using System;
using System.ComponentModel.DataAnnotations;

namespace hatmaker_team2.Models
{
    public class SalesViewModel
    {
        public DateOnly DeliveryDate { get; set; }
        public double TotalSales { get; set; }
    }

    public class MonthlySalesViewModel
    {
        public string YearMonth { get; set; }
        public string MonthName { get; set; }
        public int OrderCount { get; set; }
        public double TotalSales { get; set; }
    }

    public class QuarterlySalesViewModel
    {
        public string YearQuarter { get; set; }
        public string QuarterName { get; set; }
        public int OrderCount { get; set; }
        public double TotalSales { get; set; }
    }

    public class YearlySalesViewModel
    {
        public string Year { get; set; }
        public int OrderCount { get; set; }
        public double TotalSales { get; set; }
    }

    public class SalesDashboardViewModel
    {
        public double TotalCompletedSales { get; set; }
        public int CompletedOrderCount { get; set; }
        public double AverageOrderValue { get; set; }
        public List<MonthlySalesViewModel> MonthlySales { get; set; }
        public List<QuarterlySalesViewModel> QuarterlySales { get; set; }
        public List<YearlySalesViewModel> YearlySales { get; set; }
        public List<Order> RecentCompletedOrders { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
