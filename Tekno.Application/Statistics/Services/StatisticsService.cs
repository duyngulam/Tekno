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

    try
    {
        // FIX: Don't use Task.WhenAll with DbContext - chạy tuần tự để tránh DbContext threading issue
        // DbContext không thread-safe khi nhiều async operation chạy đồng thời
        var overview = await _statisticsRepository.GetOverviewStatisticsAsync(startDate, endDate);
        var topProducts = await _statisticsRepository.GetTopSoldProductsAsync(startDate, endDate, filter.TopCount);
        var categoryRevenue = await _statisticsRepository.GetCategoryRevenueAsync(startDate, endDate);
        var topCustomers = await _statisticsRepository.GetTopCustomersAsync(startDate, endDate, filter.TopCount);
        var recentOrders = await _statisticsRepository.GetRecentOrdersAsync(10);
        var productPerformance = await _statisticsRepository.GetProductPerformanceAsync();
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
        /// Get revenue chart data
        /// </summary>
        private async Task<RevenueChartDto> GetRevenueChartDataAsync(DateTime startDate, DateTime endDate)
{
    // FIX: Chạy tuần tự thay vì Task.WhenAll để tránh DbContext threading issue
    var daily = await _statisticsRepository.GetDailyRevenueAsync(startDate, endDate);
    var weekly = await _statisticsRepository.GetWeeklyRevenueAsync(startDate, endDate);
    var monthly = await _statisticsRepository.GetMonthlyRevenueAsync(startDate, endDate);

    return new RevenueChartDto
    {
        Daily = daily,
        Weekly = weekly,
        Monthly = monthly
    };
}

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
