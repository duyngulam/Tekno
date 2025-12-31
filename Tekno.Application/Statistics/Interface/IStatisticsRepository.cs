using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Statistics.DTOs;

namespace Tekno.Application.Statistics.Interface
{
    public interface IStatisticsRepository
    {
        // Overview statistics
        Task<OverviewStatisticsDto> GetOverviewStatisticsAsync(DateTime startDate, DateTime endDate);
        
        // Top products
        Task<List<TopProductDto>> GetTopSoldProductsAsync(DateTime startDate, DateTime endDate, int topCount);
        
        // Category revenue
        Task<List<CategoryRevenueDto>> GetCategoryRevenueAsync(DateTime startDate, DateTime endDate);
        
        // Top customers
        Task<List<TopCustomerDto>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int topCount);
        
// Revenue chart data - Standard range queries
Task<List<ChartDataPointDto>> GetDailyRevenueAsync(DateTime startDate, DateTime endDate);
Task<List<ChartDataPointDto>> GetWeeklyRevenueAsync(DateTime startDate, DateTime endDate);
Task<List<ChartDataPointDto>> GetMonthlyRevenueAsync(DateTime startDate, DateTime endDate);


        // Convenience methods for "last N days/weeks/months" scenarios
        Task<List<ChartDataPointDto>> GetLastNDaysRevenueAsync(int numberOfDays);
        Task<List<ChartDataPointDto>> GetLastNWeeksRevenueAsync(int numberOfWeeks);
        Task<List<ChartDataPointDto>> GetLastNMonthsRevenueAsync(int numberOfMonths);
        
        // Recent orders
        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 10);
        
        // Product performance
        Task<ProductPerformanceDto> GetProductPerformanceAsync();
    }
}
