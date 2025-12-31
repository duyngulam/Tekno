using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Statistics.DTOs;
using Tekno.Application.Statistics.Interface;

namespace Tekno.Application.Statistics.Services
{
    public class StatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ICacheService _cacheService;
        private readonly IAppLogger<StatisticsService> _logger;

        // Cache TTL for statistics (shorter for real-time data)
        private static readonly TimeSpan StatsCacheTtl = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan OverviewCacheTtl = TimeSpan.FromSeconds(15); // More frequent updates for overview

        public StatisticsService(
            IStatisticsRepository statisticsRepository,
            ICacheService cacheService,
            IAppLogger<StatisticsService> logger)
        {
            _statisticsRepository = statisticsRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete admin statistics dashboard
        /// </summary>
/// <summary>
/// Get complete admin statistics dashboard
/// </summary>
public async Task<AdminStatisticsDto> GetAdminStatisticsAsync(StatisticsFilterDto filter)
{
    // Validate filter
    filter ??= new StatisticsFilterDto();
    if (filter.TopCount < 5) filter.TopCount = 10;
    if (filter.TopCount > 50) filter.TopCount = 50;

    var (startDate, endDate) = GetDateRange(filter);
    var cacheKey = $"admin:stats:{filter.Period}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{filter.TopCount}";

    // Check cache first
    var cached = await _cacheService.GetAsync<AdminStatisticsDto>(cacheKey);
    if (cached != null)
    {
        _logger.LogInformation("Retrieved admin statistics from cache for period {Period}", filter.Period);
        return cached;
    }

    _logger.LogInformation("Generating admin statistics for period {StartDate} to {EndDate}", startDate, endDate);

            // Sequential awaits to avoid multiple concurrent EF queries on same DbContext
            var overview = await _statisticsRepository.GetOverviewStatisticsAsync(startDate, endDate);
            var topProducts = await _statisticsRepository.GetTopSoldProductsAsync(startDate, endDate, filter.TopCount);
            var categoryRevenue = await _statisticsRepository.GetCategoryRevenueAsync(startDate, endDate);
            var topCustomers = await _statisticsRepository.GetTopCustomersAsync(startDate, endDate, filter.TopCount);
            var recentOrders = await _statisticsRepository.GetRecentOrdersAsync(10);
            var productPerformance = await _statisticsRepository.GetProductPerformanceAsync();

            // Fetch revenue chart data sequentially as well
            var revenueChart = await GetRevenueChartDataAsync(startDate, endDate);

            var statistics = new AdminStatisticsDto
            {
                Overview = overview,
                TopSoldProducts = topProducts,
                CategoryRevenue = categoryRevenue,
                TopCustomers = topCustomers,
                RevenueChart = revenueChart,
                RecentOrders = recentOrders,
                ProductPerformance = productPerformance
            };

        // Cache the results
        await _cacheService.SetAsync(cacheKey, statistics, StatsCacheTtl);

        _logger.LogInformation("Admin statistics generated and cached successfully");

        return statistics;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating admin statistics");
        throw;
    }
}

        /// <summary>
        /// Get only overview statistics (for real-time updates)
        /// </summary>
        public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync(DateRangeFilter period)
        {
            var (startDate, endDate) = GetDateRange(new StatisticsFilterDto { Period = period });
            var cacheKey = $"admin:stats:overview:{period}:{startDate:yyyyMMdd}";

            return await _cacheService.CacheOrGetAsync(
                cacheKey,
                () => _statisticsRepository.GetOverviewStatisticsAsync(startDate, endDate),
                OverviewCacheTtl
            );
        }

        /// <summary>
        /// Get top sold products for a period
        /// </summary>
        public async Task<List<TopProductDto>> GetTopSoldProductsAsync(StatisticsFilterDto filter)
        {
            var (startDate, endDate) = GetDateRange(filter);
            var cacheKey = $"admin:stats:top-products:{filter.Period}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{filter.TopCount}";

            return await _cacheService.CacheOrGetAsync(cacheKey, () => _statisticsRepository.GetTopSoldProductsAsync(startDate, endDate, filter.TopCount), StatsCacheTtl);
        }

        /// <summary>
        /// Get category revenue for a period
        /// </summary>
        public async Task<List<CategoryRevenueDto>> GetCategoryRevenueAsync(StatisticsFilterDto filter)
        {
            var (startDate, endDate) = GetDateRange(filter);
            var cacheKey = $"admin:stats:category-revenue:{filter.Period}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}";

            return await _cacheService.CacheOrGetAsync(cacheKey, () => _statisticsRepository.GetCategoryRevenueAsync(startDate, endDate), StatsCacheTtl);
        }

        /// <summary>
        /// Get top customers for a period
        /// </summary>
        public async Task<List<TopCustomerDto>> GetTopCustomersAsync(StatisticsFilterDto filter)
        {
            var (startDate, endDate) = GetDateRange(filter);
            var cacheKey = $"admin:stats:top-customers:{filter.Period}:{startDate:yyyyMMdd}:{endDate:yyyyMMdd}:{filter.TopCount}";

            return await _cacheService.CacheOrGetAsync(cacheKey, () => _statisticsRepository.GetTopCustomersAsync(startDate, endDate, filter.TopCount), StatsCacheTtl);
        }

        /// <summary>
        /// Get revenue chart data
        /// </summary>
        public async Task<RevenueChartDto> GetRevenueChartAsync(StatisticsFilterDto filter)
        {
            var (startDate, endDate) = GetDateRange(filter);
            return await GetRevenueChartDataAsync(startDate, endDate);
        }

        /// <summary>
        /// Get recent orders
        /// </summary>
        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 10)
        {
            var cacheKey = $"admin:stats:recent-orders:{count}";
            return await _cacheService.CacheOrGetAsync(cacheKey, () => _statisticsRepository.GetRecentOrdersAsync(count), StatsCacheTtl);
        }

        /// <summary>
        /// Get product performance
        /// </summary>
        public async Task<ProductPerformanceDto> GetProductPerformanceAsync()
        {
            var cacheKey = "admin:stats:product-performance";
            return await _cacheService.CacheOrGetAsync(cacheKey, () => _statisticsRepository.GetProductPerformanceAsync(), StatsCacheTtl);
        }

        /// <summary>
        /// Get revenue chart data (internal)
        /// </summary>
        private async Task<RevenueChartDto> GetRevenueChartDataAsync(DateTime startDate, DateTime endDate)
        {
            // Sequential to avoid concurrent DbContext usage
            var daily = await _statistics_repository_GetDaily(startDate, endDate);
            var weekly = await _statistics_repository_GetWeekly(startDate, endDate);
            var monthly = await _statistics_repository_GetMonthly(startDate, endDate);

            return new RevenueChartDto
            {
                Daily = daily,
                Weekly = weekly,
                Monthly = monthly
            };
        }

        // Helper wrappers to call repository (keeps naming consistent and allows centralized AsNoTracking in repo)
        private Task<List<ChartDataPointDto>> _statistics_repository_GetDaily(DateTime s, DateTime e) => _statisticsRepository.GetDailyRevenueAsync(s, e);
        private Task<List<ChartDataPointDto>> _statistics_repository_GetWeekly(DateTime s, DateTime e) => _statisticsRepository.GetWeeklyRevenueAsync(s, e);
        private Task<List<ChartDataPointDto>> _statistics_repository_GetMonthly(DateTime s, DateTime e) => _statisticsRepository.GetMonthlyRevenueAsync(s, e);

        /// <summary>
        /// Calculate date range from filter
        /// </summary>
private (DateTime startDate, DateTime endDate) GetDateRange(StatisticsFilterDto filter)
{
    var now = DateTime.UtcNow;
    var today = now.Date; // This will be Unspecified, need to fix
    today = DateTime.SpecifyKind(today, DateTimeKind.Utc); // FIX: Specify as UTC

    if (filter.Period == DateRangeFilter.Custom && filter.StartDate.HasValue && filter.EndDate.HasValue)
    {
        var startDate = filter.StartDate.Value;
        var endDate = filter.EndDate.Value;
        
        // Ensure both are UTC
        if (startDate.Kind == DateTimeKind.Unspecified)
            startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        if (endDate.Kind == DateTimeKind.Unspecified)
            endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            
        return (startDate.Date, endDate.Date.AddDays(1).AddSeconds(-1));
    }

    return filter.Period switch
    {
        DateRangeFilter.Today => (today, now),
        DateRangeFilter.Yesterday => (today.AddDays(-1), today.AddSeconds(-1)),
        DateRangeFilter.Last7Days => (today.AddDays(-7), now),
        DateRangeFilter.Last30Days => (today.AddDays(-30), now),
        DateRangeFilter.ThisWeek => (GetStartOfWeek(today), now),
        DateRangeFilter.LastWeek => (GetStartOfWeek(today.AddDays(-7)), GetStartOfWeek(today).AddSeconds(-1)),
        DateRangeFilter.ThisMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc), now),
        DateRangeFilter.LastMonth => (new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-1), new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(-1)),
        DateRangeFilter.ThisQuarter => (GetStartOfQuarter(now), now),
        DateRangeFilter.LastQuarter => (GetStartOfQuarter(now).AddMonths(-3), GetStartOfQuarter(now).AddSeconds(-1)),
        DateRangeFilter.ThisYear => (new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc), now),
        DateRangeFilter.LastYear => (new DateTime(now.Year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(-1)),
        _ => (today.AddDays(-7), now)
    };
}

private DateTime GetStartOfWeek(DateTime date)
{
    var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
    var startOfWeek = date.AddDays(-1 * diff).Date;
    return DateTime.SpecifyKind(startOfWeek, DateTimeKind.Utc);
}

private DateTime GetStartOfQuarter(DateTime date)
{
    var quarterMonth = ((date.Month - 1) / 3) * 3 + 1;
    var startOfQuarter = new DateTime(date.Year, quarterMonth, 1);
    return DateTime.SpecifyKind(startOfQuarter, DateTimeKind.Utc);
}

        /// <summary>
        /// Invalidate statistics cache (call when orders are updated)
        /// </summary>
        public async Task InvalidateStatisticsCacheAsync()
        {
            // In a real implementation, you'd want to delete all cache keys matching the pattern
            // For now, we'll just log it and let cache expire naturally
            _logger.LogInformation("Statistics cache invalidation requested - cache will expire naturally");
        }

        /// <summary>
        /// Get daily revenue for last N days (convenience method)
        /// </summary>
        public async Task<List<ChartDataPointDto>> GetLastNDaysRevenueAsync(int numberOfDays)
        {
            var cacheKey = $"admin:revenue:last-days:{numberOfDays}";
            return await _cacheService.CacheOrGetAsync(
                cacheKey,
                () => _statisticsRepository.GetLastNDaysRevenueAsync(numberOfDays),
                StatsCacheTtl
            );
        }

        /// <summary>
        /// Get weekly revenue for last N weeks (convenience method)
        /// </summary>
        public async Task<List<ChartDataPointDto>> GetLastNWeeksRevenueAsync(int numberOfWeeks)
        {
            var cacheKey = $"admin:revenue:last-weeks:{numberOfWeeks}";
            return await _cacheService.CacheOrGetAsync(
                cacheKey,
                () => _statisticsRepository.GetLastNWeeksRevenueAsync(numberOfWeeks),
                StatsCacheTtl
            );
        }

        /// <summary>
        /// Get monthly revenue for last N months (convenience method)
        /// </summary>
        public async Task<List<ChartDataPointDto>> GetLastNMonthsRevenueAsync(int numberOfMonths)
        {
            var cacheKey = $"admin:revenue:last-months:{numberOfMonths}";
            return await _cacheService.CacheOrGetAsync(
                cacheKey,
                () => _statisticsRepository.GetLastNMonthsRevenueAsync(numberOfMonths),
                StatsCacheTtl
            );
        }
    }
}
