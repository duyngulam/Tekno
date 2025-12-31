using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tekno.Api.Commons.Responses;
using Tekno.Application.Statistics.DTOs;
using Tekno.Application.Statistics.Services;

namespace Tekno.Api.Controllers.admin
{
    /// <summary>
    /// Admin statistics and analytics endpoints
    /// </summary>
    [ApiController]
    [Route("api/admin/statistics")]
    [Authorize(Roles = "Admin")]
    public class AdminStatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;
        private readonly ILogger<AdminStatisticsController> _logger;

        public AdminStatisticsController(
            StatisticsService statisticsService,
            ILogger<AdminStatisticsController> logger)
        {
            _statisticsService = statisticsService;
            _logger = logger;
        }

        /// <summary>
        /// Get complete admin dashboard statistics
        /// </summary>
        /// <remarks>
        /// Returns comprehensive statistics including:
        /// - Overview metrics (revenue, orders, customers, AOV)
        /// - Top selling products
        /// - Revenue by category
        /// - Top customers
        /// - Revenue charts (daily, weekly, monthly)
        /// - Recent orders
        /// - Product performance metrics
        /// 
        /// Supports multiple date range filters:
        /// - Today, Yesterday
        /// - Last7Days, Last30Days
        /// - ThisWeek, LastWeek
        /// - ThisMonth, LastMonth
        /// - ThisQuarter, LastQuarter
        /// - ThisYear, LastYear
        /// - Custom (requires startDate and endDate)
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/admin/statistics?period=Last7Days&amp;topCount=10
        ///     GET /api/admin/statistics?period=ThisMonth
        ///     GET /api/admin/statistics?period=Custom&amp;startDate=2025-01-01&amp;endDate=2025-01-31
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Last7Days)</param>
        /// <param name="startDate">Start date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="endDate">End date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="topCount">Number of top items to return (default: 10, max: 50)</param>
        /// <response code="200">Returns the statistics data</response>
        /// <response code="400">Invalid date range or parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<AdminStatisticsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Last7Days,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int topCount = 10)
        {
            if (topCount < 1 || topCount > 50)
            {
                return BadRequest(ApiResponse<object>.Fail("topCount must be between 1 and 50"));
            }

            if (period == DateRangeFilter.Custom)
            {
                if (!startDate.HasValue || !endDate.HasValue)
                {
                    return BadRequest(ApiResponse<object>.Fail("startDate and endDate are required for custom period"));
                }

                if (startDate.Value > endDate.Value)
                {
                    return BadRequest(ApiResponse<object>.Fail("startDate must be before endDate"));
                }
            }

            var filter = new StatisticsFilterDto
            {
                Period = period,
                StartDate = startDate,
                EndDate = endDate,
                TopCount = topCount
            };

            _logger.LogInformation("Fetching admin statistics for period: {Period}, topCount: {TopCount}", period, topCount);

            var statistics = await _statisticsService.GetAdminStatisticsAsync(filter);

            return Ok(ApiResponse<AdminStatisticsDto>.Ok(statistics, $"Statistics for {period} retrieved successfully"));
        }

        /// <summary>
        /// Get only overview statistics (lightweight endpoint for real-time updates)
        /// </summary>
        /// <remarks>
        /// Returns only the overview metrics:
        /// - Total revenue (with growth %)
        /// - Total orders (with growth %)
        /// - Total customers (with growth %)
        /// - Average order value (with growth %)
        /// - Order status breakdown
        /// - Completion and cancellation rates
        /// 
        /// This endpoint is optimized for frequent polling (e.g., every 30 seconds)
        /// to provide real-time dashboard updates without loading all statistics.
        /// 
        /// Cache TTL: 2 minutes
        /// 
        /// Sample request:
        /// 
        ///     GET /api/admin/statistics/overview?period=Today
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Today)</param>
        /// <response code="200">Returns the overview statistics</response>
        [HttpGet("overview")]
        [ProducesResponseType(typeof(ApiResponse<OverviewStatisticsDto>), 200)]
        public async Task<IActionResult> GetOverview(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Today)
        {
            var overview = await _statisticsService.GetOverviewStatisticsAsync(period);
            return Ok(ApiResponse<OverviewStatisticsDto>.Ok(overview));
        }

        /// <summary>
        /// Get top sold products for a period
        /// </summary>
        /// <remarks>
        /// Returns a list of the top selling products including:
        /// - Product details (ID, name, SKU)
        /// - Total quantity sold
        /// - Total revenue
        /// - Average discount
        /// 
        /// Supports multiple date range filters:
        /// - Today, Yesterday
        /// - Last7Days, Last30Days
        /// - ThisWeek, LastWeek
        /// - ThisMonth, LastMonth
        /// - ThisQuarter, LastQuarter
        /// - ThisYear, LastYear
        /// - Custom (requires startDate and endDate)
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/admin/statistics/top-products?period=Last7Days&amp;topCount=10
        ///     GET /api/admin/statistics/top-products?period=ThisMonth
        ///     GET /api/admin/statistics/top-products?period=Custom&amp;startDate=2025-01-01&amp;endDate=2025-01-31
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Last7Days)</param>
        /// <param name="startDate">Start date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="endDate">End date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="topCount">Number of top items to return (default: 10, max: 50)</param>
        /// <response code="200">Returns the top products data</response>
        /// <response code="400">Invalid date range or parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet("top-products")]
        [ProducesResponseType(typeof(ApiResponse<List<TopProductDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Last7Days,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int topCount = 10)
        {
            if (topCount < 1 || topCount > 50)
                return BadRequest(ApiResponse<object>.Fail("topCount must be between 1 and 50"));

            var filter = new StatisticsFilterDto { Period = period, StartDate = startDate, EndDate = endDate, TopCount = topCount };
            var data = await _statisticsService.GetTopSoldProductsAsync(filter);
            return Ok(ApiResponse<List<TopProductDto>>.Ok(data));
        }

        /// <summary>
        /// Get category revenue for a period
        /// </summary>
        /// <remarks>
        /// Returns revenue data grouped by category including:
        /// - Category details (ID, name)
        /// - Total revenue
        /// - Total orders
        /// - Total products sold
        /// 
        /// Supports multiple date range filters:
        /// - Today, Yesterday
        /// - Last7Days, Last30Days
        /// - ThisWeek, LastWeek
        /// - ThisMonth, LastMonth
        /// - ThisQuarter, LastQuarter
        /// - ThisYear, LastYear
        /// - Custom (requires startDate and endDate)
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/admin/statistics/category-revenue?period=Last7Days
        ///     GET /api/admin/statistics/category-revenue?period=ThisMonth
        ///     GET /api/admin/statistics/category-revenue?period=Custom&amp;startDate=2025-01-01&amp;endDate=2025-01-31
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Last7Days)</param>
        /// <param name="startDate">Start date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="endDate">End date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <response code="200">Returns the category revenue data</response>
        /// <response code="400">Invalid date range or parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet("category-revenue")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryRevenueDto>>), 200)]
        public async Task<IActionResult> GetCategoryRevenue(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Last7Days,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var filter = new StatisticsFilterDto { Period = period, StartDate = startDate, EndDate = endDate };
            var data = await _statisticsService.GetCategoryRevenueAsync(filter);
            return Ok(ApiResponse<List<CategoryRevenueDto>>.Ok(data));
        }

        /// <summary>
        /// Get top customers for a period
        /// </summary>
        /// <remarks>
        /// Returns a list of the top customers including:
        /// - Customer details (ID, name, email)
        /// - Total orders
        /// - Total revenue
        /// - Average order value
        /// 
        /// Supports multiple date range filters:
        /// - Today, Yesterday
        /// - Last7Days, Last30Days
        /// - ThisWeek, LastWeek
        /// - ThisMonth, LastMonth
        /// - ThisQuarter, LastQuarter
        /// - ThisYear, LastYear
        /// - Custom (requires startDate and endDate)
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/admin/statistics/top-customers?period=Last7Days&amp;topCount=10
        ///     GET /api/admin/statistics/top-customers?period=ThisMonth
        ///     GET /api/admin/statistics/top-customers?period=Custom&amp;startDate=2025-01-01&amp;endDate=2025-01-31
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Last7Days)</param>
        /// <param name="startDate">Start date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="endDate">End date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="topCount">Number of top items to return (default: 10, max: 50)</param>
        /// <response code="200">Returns the top customers data</response>
        /// <response code="400">Invalid date range or parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet("top-customers")]
        [ProducesResponseType(typeof(ApiResponse<List<TopCustomerDto>>), 200)]
        public async Task<IActionResult> GetTopCustomers(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Last7Days,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int topCount = 10)
        {
            if (topCount < 1 || topCount > 50)
                return BadRequest(ApiResponse<object>.Fail("topCount must be between 1 and 50"));

            var filter = new StatisticsFilterDto { Period = period, StartDate = startDate, EndDate = endDate, TopCount = topCount };
            var data = await _statisticsService.GetTopCustomersAsync(filter);
            return Ok(ApiResponse<List<TopCustomerDto>>.Ok(data));
        }

        /// <summary>
        /// Get revenue chart data (daily/weekly/monthly) for a period
        /// </summary>
        /// <remarks>
        /// Returns revenue chart data including:
        /// - Daily, weekly, or monthly revenue totals
        /// - Comparison with previous period
        /// - Growth percentage
        /// 
        /// Supports multiple date range filters:
        /// - Today, Yesterday
        /// - Last7Days, Last30Days
        /// - ThisWeek, LastWeek
        /// - ThisMonth, LastMonth
        /// - ThisQuarter, LastQuarter
        /// - ThisYear, LastYear
        /// - Custom (requires startDate and endDate)
        /// 
        /// Sample requests:
        /// 
        ///     GET /api/admin/statistics/revenue-chart?period=Last7Days
        ///     GET /api/admin/statistics/revenue-chart?period=ThisMonth
        ///     GET /api/admin/statistics/revenue-chart?period=Custom&amp;startDate=2025-01-01&amp;endDate=2025-01-31
        /// 
        /// </remarks>
        /// <param name="period">Date range filter (default: Last7Days)</param>
        /// <param name="startDate">Start date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <param name="endDate">End date for custom period (ISO format: yyyy-MM-dd)</param>
        /// <response code="200">Returns the revenue chart data</response>
        /// <response code="400">Invalid date range or parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet("revenue-chart")]
        [ProducesResponseType(typeof(ApiResponse<RevenueChartDto>), 200)]
        public async Task<IActionResult> GetRevenueChart(
            [FromQuery] DateRangeFilter period = DateRangeFilter.Last7Days,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var filter = new StatisticsFilterDto { Period = period, StartDate = startDate, EndDate = endDate };
            var data = await _statisticsService.GetRevenueChartAsync(filter);
            return Ok(ApiResponse<RevenueChartDto>.Ok(data));
        }

        /// <summary>
        /// Get recent orders
        /// </summary>
        /// <remarks>
        /// Returns a list of recent orders including:
        /// - Order details (ID, date, customer, total amount)
        /// - Product details for each order (SKU, name, quantity, price)
        /// - Order status
        /// 
        /// Supports pagination:
        /// - page: Page number (default: 1)
        /// - pageSize: Number of orders per page (default: 10, max: 100)
        /// 
        /// Sample request:
        /// 
        ///     GET /api/admin/statistics/recent-orders?page=1&pageSize=10
        /// 
        /// </remarks>
        /// <param name="count">Number of recent orders to return (default: 10, max: 100)</param>
        /// <response code="200">Returns the recent orders data</response>
        /// <response code="400">Invalid parameters</response>
        /// <response code="401">Unauthorized - Authentication required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        [HttpGet("recent-orders")]
        [ProducesResponseType(typeof(ApiResponse<List<RecentOrderDto>>), 200)]
        public async Task<IActionResult> GetRecentOrders([FromQuery] int count = 10)
        {
            if (count < 1 || count > 100) return BadRequest(ApiResponse<object>.Fail("count must be between 1 and 100"));
            var data = await _statisticsService.GetRecentOrdersAsync(count);
            return Ok(ApiResponse<List<RecentOrderDto>>.Ok(data));
        }

        /// <summary>
        /// Get product performance metrics
        /// </summary>
        /// <remarks>
        /// Returns key product performance metrics including:
        /// - Total products
        /// - Active products
        /// - Inactive products
        /// - Products with low stock
        /// - Products with high returns
        /// 
        /// Sample request:
        /// 
        ///     GET /api/admin/statistics/product-performance
        /// 
        /// </remarks>
        /// <response code="200">Returns the product performance data</response>
        [HttpGet("product-performance")]
        [ProducesResponseType(typeof(ApiResponse<ProductPerformanceDto>), 200)]
        public async Task<IActionResult> GetProductPerformance()
        {
            var data = await _statisticsService.GetProductPerformanceAsync();
            return Ok(ApiResponse<ProductPerformanceDto>.Ok(data));
        }

        /// <summary>
        /// Get available date range filter options
        /// </summary>
        /// <remarks>
        /// Returns a list of all available date range filters that can be used
        /// in the statistics endpoints. Useful for building UI dropdown/filter controls.
        /// </remarks>
        /// <response code="200">Returns list of available filters</response>
        [HttpGet("filters")]
        [AllowAnonymous] // Allow this for UI building
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public IActionResult GetAvailableFilters()
        {
            var filters = new
            {
                dateRanges = new[]
                {
                    new { value = "Today", label = "Today", description = "Orders from today" },
                    new { value = "Yesterday", label = "Yesterday", description = "Orders from yesterday" },
                    new { value = "Last7Days", label = "Last 7 Days", description = "Orders from the last 7 days" },
                    new { value = "Last30Days", label = "Last 30 Days", description = "Orders from the last 30 days" },
                    new { value = "ThisWeek", label = "This Week", description = "Orders from the current week" },
                    new { value = "LastWeek", label = "Last Week", description = "Orders from last week" },
                    new { value = "ThisMonth", label = "This Month", description = "Orders from the current month" },
                    new { value = "LastMonth", label = "Last Month", description = "Orders from last month" },
                    new { value = "ThisQuarter", label = "This Quarter", description = "Orders from the current quarter" },
                    new { value = "LastQuarter", label = "Last Quarter", description = "Orders from last quarter" },
                    new { value = "ThisYear", label = "This Year", description = "Orders from the current year" },
                    new { value = "LastYear", label = "Last Year", description = "Orders from last year" },
                    new { value = "Custom", label = "Custom Range", description = "Custom date range (requires startDate and endDate)" }
                },
                defaultPeriod = "Last7Days",
                maxTopCount = 50,
                defaultTopCount = 10,
                cacheTtl = "5 minutes (overview: 2 minutes)"
            };

            return Ok(ApiResponse<object>.Ok(filters));
        }

        /// <summary>
        /// Invalidate statistics cache (call after bulk operations)
        /// </summary>
        /// <remarks>
        /// Manually invalidate the statistics cache. Useful after:
        /// - Bulk order updates
        /// - Data imports
        /// - Manual database changes
        /// 
        /// Cache will expire naturally after 5 minutes anyway, but this allows
        /// immediate refresh when needed.
        /// </remarks>
        /// <response code="200">Cache invalidation initiated</response>
        [HttpPost("invalidate-cache")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> InvalidateCache()
        {
            await _statisticsService.InvalidateStatisticsCacheAsync();
            return Ok(ApiResponse<object>.Ok(null, "Statistics cache invalidation initiated"));
        }

        /// <summary>
        /// Get revenue chart for last N days (convenience endpoint)
        /// </summary>
        /// <remarks>
        /// Returns daily revenue data for the last N days.
        /// 
        /// Examples:
        /// - GET /api/admin/statistics/revenue/last-days/7 - Last 7 days
        /// - GET /api/admin/statistics/revenue/last-days/30 - Last 30 days
        /// 
        /// Useful for quick charts without calculating date ranges.
        /// </remarks>
        /// <param name="numberOfDays">Number of days to retrieve (1-365)</param>
        /// <response code="200">Returns daily revenue data</response>
        /// <response code="400">Invalid number of days</response>
        [HttpGet("revenue/last-days/{numberOfDays:int}")]
        [ProducesResponseType(typeof(ApiResponse<List<ChartDataPointDto>>), 200)]
        public async Task<IActionResult> GetLastNDaysRevenue(int numberOfDays)
        {
            if (numberOfDays < 1 || numberOfDays > 365)
            {
                return BadRequest(ApiResponse<object>.Fail("numberOfDays must be between 1 and 365"));
            }

            var data = await _statisticsService.GetLastNDaysRevenueAsync(numberOfDays);
            return Ok(ApiResponse<List<ChartDataPointDto>>.Ok(data, $"Daily revenue for last {numberOfDays} days"));
        }

        /// <summary>
        /// Get revenue chart for last N weeks (convenience endpoint)
        /// </summary>
        /// <remarks>
        /// Returns weekly revenue data for the last N weeks.
        /// 
        /// Examples:
        /// - GET /api/admin/statistics/revenue/last-weeks/4 - Last 4 weeks
        /// - GET /api/admin/statistics/revenue/last-weeks/12 - Last 12 weeks (3 months)
        /// </remarks>
        /// <param name="numberOfWeeks">Number of weeks to retrieve (1-52)</param>
        /// <response code="200">Returns weekly revenue data</response>
        /// <response code="400">Invalid number of weeks</response>
        [HttpGet("revenue/last-weeks/{numberOfWeeks:int}")]
        [ProducesResponseType(typeof(ApiResponse<List<ChartDataPointDto>>), 200)]
        public async Task<IActionResult> GetLastNWeeksRevenue(int numberOfWeeks)
        {
            if (numberOfWeeks < 1 || numberOfWeeks > 52)
            {
                return BadRequest(ApiResponse<object>.Fail("numberOfWeeks must be between 1 and 52"));
            }

            var data = await _statisticsService.GetLastNWeeksRevenueAsync(numberOfWeeks);
            return Ok(ApiResponse<List<ChartDataPointDto>>.Ok(data, $"Weekly revenue for last {numberOfWeeks} weeks"));
        }

        /// <summary>
        /// Get revenue chart for last N months (convenience endpoint)
        /// </summary>
        /// <remarks>
        /// Returns monthly revenue data for the last N months.
        /// 
        /// Examples:
        /// - GET /api/admin/statistics/revenue/last-months/6 - Last 6 months
        /// - GET /api/admin/statistics/revenue/last-months/12 - Last 12 months (1 year)
        /// </remarks>
        /// <param name="numberOfMonths">Number of months to retrieve (1-24)</param>
        /// <response code="200">Returns monthly revenue data</response>
        /// <response code="400">Invalid number of months</response>
        [HttpGet("revenue/last-months/{numberOfMonths:int}")]
        [ProducesResponseType(typeof(ApiResponse<List<ChartDataPointDto>>), 200)]
        public async Task<IActionResult> GetLastNMonthsRevenue(int numberOfMonths)
        {
            if (numberOfMonths < 1 || numberOfMonths > 24)
            {
                return BadRequest(ApiResponse<object>.Fail("numberOfMonths must be between 1 and 24"));
            }

            var data = await _statisticsService.GetLastNMonthsRevenueAsync(numberOfMonths);
            return Ok(ApiResponse<List<ChartDataPointDto>>.Ok(data, $"Monthly revenue for last {numberOfMonths} months"));
        }
    }
}