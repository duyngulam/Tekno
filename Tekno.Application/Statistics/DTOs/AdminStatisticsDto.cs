using System;
using System.Collections.Generic;

namespace Tekno.Application.Statistics.DTOs
{
    /// <summary>
    /// Main statistics dashboard data
    /// </summary>
    public class AdminStatisticsDto
    {
        public OverviewStatisticsDto Overview { get; set; } = new();
        public List<TopProductDto> TopSoldProducts { get; set; } = new();
        public List<CategoryRevenueDto> CategoryRevenue { get; set; } = new();
        public List<TopCustomerDto> TopCustomers { get; set; } = new();
        public RevenueChartDto RevenueChart { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
        public ProductPerformanceDto ProductPerformance { get; set; } = new();
    }

    /// <summary>
    /// Overview metrics (KPIs)
    /// </summary>
    public class OverviewStatisticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal PreviousPeriodRevenue { get; set; }
        public double RevenueGrowthPercent { get; set; }
        
        public int TotalOrders { get; set; }
        public int PreviousPeriodOrders { get; set; }
        public double OrderGrowthPercent { get; set; }
        
        public int TotalCustomers { get; set; }
        public int NewCustomers { get; set; }
        public double CustomerGrowthPercent { get; set; }
        
        public decimal AverageOrderValue { get; set; }
        public decimal PreviousAverageOrderValue { get; set; }
        public double AovGrowthPercent { get; set; }
        
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        
        public double OrderCompletionRate { get; set; }
        public double OrderCancellationRate { get; set; }
    }

    /// <summary>
    /// Top selling products
    /// </summary>
    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSlug { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal AveragePrice { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    /// <summary>
    /// Revenue by category
    /// </summary>
    public class CategoryRevenueDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategorySlug { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public int ProductsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double RevenuePercentage { get; set; }
    }

    /// <summary>
    /// Top customers by spending
    /// </summary>
    public class TopCustomerDto
    {
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public string CustomerSegment { get; set; } = string.Empty; // VIP, Regular, New
    }

    /// <summary>
    /// Revenue chart data for graphs
    /// </summary>
    public class RevenueChartDto
    {
        public List<ChartDataPointDto> Daily { get; set; } = new();
        public List<ChartDataPointDto> Weekly { get; set; } = new();
        public List<ChartDataPointDto> Monthly { get; set; } = new();
    }

    public class ChartDataPointDto
    {
        public string Label { get; set; } = string.Empty; // Date/Week/Month label
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    /// <summary>
    /// Recent orders for activity feed
    /// </summary>
    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Product performance metrics
    /// </summary>
    public class ProductPerformanceDto
    {
        public int TotalProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public List<LowStockProductDto> LowStockAlerts { get; set; } = new();
        public int ProductsWithNoSales { get; set; }
        public double AverageProductRating { get; set; }
    }

    public class LowStockProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int VariantId { get; set; }
        public string VariantSku { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int RecommendedStock { get; set; }
    }

    /// <summary>
    /// Filter options for statistics
    /// </summary>
    public class StatisticsFilterDto
    {
        public DateRangeFilter Period { get; set; } = DateRangeFilter.Last7Days;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int TopCount { get; set; } = 10;
    }

    public enum DateRangeFilter
    {
        Today,
        Yesterday,
        Last7Days,
        Last30Days,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        ThisQuarter,
        LastQuarter,
        ThisYear,
        LastYear,
        Custom
    }

    public class DateRangeOptionDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class StatisticsFilterOptionsDto
    {
        public List<DateRangeOptionDto> DateRanges { get; set; } = new();
        public string DefaultPeriod { get; set; } = string.Empty;
        public int MaxTopCount { get; set; }
        public int DefaultTopCount { get; set; }
        public string CacheTtl { get; set; } = string.Empty;
    }
}
