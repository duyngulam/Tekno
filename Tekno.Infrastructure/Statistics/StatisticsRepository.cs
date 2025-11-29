using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Statistics.DTOs;
using Tekno.Application.Statistics.Interface;
using Tekno.Infrastructure.Persistence;
using OrderEntity = Tekno.Domain.Order.Order;
using OrderStatus = Tekno.Domain.Order.OrderStatus;

namespace Tekno.Infrastructure.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly AppDbContext _context;

        public StatisticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OverviewStatisticsDto> GetOverviewStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            // Current period orders
            var orders = await _context.Set<OrderEntity>()
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();

            var completedOrders = orders.Where(o => o.Status == OrderStatus.Completed).ToList();

            // Previous period for comparison
            var periodDays = (endDate - startDate).Days;
            var previousStartDate = startDate.AddDays(-periodDays);
            var previousEndDate = startDate.AddSeconds(-1);

            var previousOrders = await _context.Set<OrderEntity>()
                .Where(o => o.CreatedAt >= previousStartDate && o.CreatedAt <= previousEndDate)
                .ToListAsync();

            var previousCompletedOrders = previousOrders.Where(o => o.Status == OrderStatus.Completed).ToList();

            // Calculate metrics
            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var previousRevenue = previousCompletedOrders.Sum(o => o.TotalAmount);
            var revenueGrowth = previousRevenue > 0 
                ? (double)((totalRevenue - previousRevenue) / previousRevenue) * 100 
                : 0;

            var totalOrders = orders.Count;
            var previousTotalOrders = previousOrders.Count;
            var orderGrowth = previousTotalOrders > 0 
                ? ((double)(totalOrders - previousTotalOrders) / previousTotalOrders) * 100 
                : 0;

            // Customer metrics
            var customerIds = orders.Select(o => o.UserId).Distinct().ToList();
            var newCustomers = await _context.Set<OrderEntity>()
                .Where(o => customerIds.Contains(o.UserId))
                .GroupBy(o => o.UserId)
                .Where(g => g.Min(o => o.CreatedAt) >= startDate && g.Min(o => o.CreatedAt) <= endDate)
                .CountAsync();

            var previousCustomerIds = previousOrders.Select(o => o.UserId).Distinct().ToList();
            var customerGrowth = previousCustomerIds.Count > 0 
                ? ((double)(customerIds.Count - previousCustomerIds.Count) / previousCustomerIds.Count) * 100 
                : 0;

            // Average order value
            var avgOrderValue = completedOrders.Any() ? completedOrders.Average(o => o.TotalAmount) : 0;
            var previousAvgOrderValue = previousCompletedOrders.Any() ? previousCompletedOrders.Average(o => o.TotalAmount) : 0;
            var aovGrowth = previousAvgOrderValue > 0 
                ? (double)((avgOrderValue - previousAvgOrderValue) / previousAvgOrderValue) * 100 
                : 0;

            // Order status counts
            var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
            var processingOrders = orders.Count(o => o.Status == OrderStatus.Processing);
            var completedOrdersCount = completedOrders.Count;
            var cancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled);

            // Rates
            var orderCompletionRate = totalOrders > 0 ? (double)completedOrdersCount / totalOrders * 100 : 0;
            var orderCancellationRate = totalOrders > 0 ? (double)cancelledOrders / totalOrders * 100 : 0;

            return new OverviewStatisticsDto
            {
                TotalRevenue = totalRevenue,
                PreviousPeriodRevenue = previousRevenue,
                RevenueGrowthPercent = Math.Round(revenueGrowth, 2),
                
                TotalOrders = totalOrders,
                PreviousPeriodOrders = previousTotalOrders,
                OrderGrowthPercent = Math.Round(orderGrowth, 2),
                
                TotalCustomers = customerIds.Count,
                NewCustomers = newCustomers,
                CustomerGrowthPercent = Math.Round(customerGrowth, 2),
                
                AverageOrderValue = Math.Round(avgOrderValue, 2),
                PreviousAverageOrderValue = Math.Round(previousAvgOrderValue, 2),
                AovGrowthPercent = Math.Round(aovGrowth, 2),
                
                PendingOrders = pendingOrders,
                ProcessingOrders = processingOrders,
                CompletedOrders = completedOrdersCount,
                CancelledOrders = cancelledOrders,
                
                OrderCompletionRate = Math.Round(orderCompletionRate, 2),
                OrderCancellationRate = Math.Round(orderCancellationRate, 2)
            };
        }

        public async Task<List<TopProductDto>> GetTopSoldProductsAsync(DateTime startDate, DateTime endDate, int topCount)
        {
            // Get completed order IDs first
            var completedOrderIds = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Select(o => o.Id)
                .ToListAsync();

            // Get order items for those orders
            var topProducts = await _context.Set<Tekno.Domain.Order.OrderItem>()
                .Where(item => completedOrderIds.Contains(item.OrderId))
                .GroupBy(item => item.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    UnitsSold = g.Sum(item => item.Quantity),
                    Revenue = g.Sum(item => item.Price * item.Quantity),
                    AveragePrice = g.Average(item => item.Price)
                })
                .OrderByDescending(x => x.UnitsSold)
                .Take(topCount)
                .ToListAsync();

            var productIds = topProducts.Select(p => p.ProductId).ToList();

            // Get product details
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Slug,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary) != null 
                        ? p.Images.First(i => i.IsPrimary).ImageUrl 
                        : null
                })
                .ToDictionaryAsync(p => p.Id);

            // Get ratings
            var ratings = await _context.Set<Domain.Review.ProductReview>()
                .Where(r => productIds.Contains(r.ProductId) && r.Status == Domain.Review.ReviewStatus.Approved)
                .GroupBy(r => r.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    TotalReviews = g.Count()
                })
                .ToDictionaryAsync(x => x.ProductId);

            return topProducts.Select(p => new TopProductDto
            {
                ProductId = p.ProductId,
                ProductName = products.ContainsKey(p.ProductId) ? products[p.ProductId].Name : "Unknown",
                ProductSlug = products.ContainsKey(p.ProductId) ? products[p.ProductId].Slug : "",
                CategoryName = products.ContainsKey(p.ProductId) ? products[p.ProductId].CategoryName : "",
                ImageUrl = products.ContainsKey(p.ProductId) ? products[p.ProductId].ImageUrl : null,
                UnitsSold = p.UnitsSold,
                Revenue = Math.Round(p.Revenue, 2),
                AveragePrice = Math.Round(p.AveragePrice, 2),
                AverageRating = ratings.ContainsKey(p.ProductId) ? Math.Round(ratings[p.ProductId].AverageRating, 1) : 0,
                TotalReviews = ratings.ContainsKey(p.ProductId) ? ratings[p.ProductId].TotalReviews : 0
            }).ToList();
        }

        public async Task<List<CategoryRevenueDto>> GetCategoryRevenueAsync(DateTime startDate, DateTime endDate)
        {
            // Get completed order IDs
            var completedOrderIds = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Select(o => o.Id)
                .ToListAsync();

            // Get category revenue by joining with products
            var categoryData = await (
                from item in _context.Set<Tekno.Domain.Order.OrderItem>()
                join product in _context.Products on item.ProductId equals product.Id
                join category in _context.Categories on product.CategoryId equals category.Id
                where completedOrderIds.Contains(item.OrderId)
                group new { item, product } by new { category.Id, category.Name, category.Slug } into g
                select new
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    CategorySlug = g.Key.Slug,
                    Revenue = g.Sum(x => x.item.Price * x.item.Quantity),
                    OrderCount = g.Select(x => x.item.OrderId).Distinct().Count(),
                    ProductsSold = g.Sum(x => x.item.Quantity)
                }
            ).OrderByDescending(x => x.Revenue)
             .ToListAsync();

            var totalRevenue = categoryData.Sum(c => c.Revenue);

            return categoryData.Select(c => new CategoryRevenueDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategorySlug = c.CategorySlug,
                Revenue = Math.Round(c.Revenue, 2),
                OrderCount = c.OrderCount,
                ProductsSold = c.ProductsSold,
                AverageOrderValue = c.OrderCount > 0 ? Math.Round(c.Revenue / c.OrderCount, 2) : 0,
                RevenuePercentage = totalRevenue > 0 ? Math.Round((double)(c.Revenue / totalRevenue * 100), 2) : 0
            }).ToList();
        }

        public async Task<List<TopCustomerDto>> GetTopCustomersAsync(DateTime startDate, DateTime endDate, int topCount)
        {
            var topCustomers = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    LastOrderDate = g.Max(o => o.CreatedAt),
                    FirstOrderDate = g.Min(o => o.CreatedAt)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(topCount)
                .ToListAsync();

            var userIds = topCustomers.Select(c => c.UserId).ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Fullname, u.Email })
                .ToDictionaryAsync(u => u.Id);

            return topCustomers.Select(c => new TopCustomerDto
            {
                UserId = c.UserId,
                CustomerName = users.ContainsKey(c.UserId) ? users[c.UserId].Fullname : "Unknown",
                Email = users.ContainsKey(c.UserId) ? users[c.UserId].Email : "",
                TotalSpent = Math.Round(c.TotalSpent, 2),
                OrderCount = c.OrderCount,
                AverageOrderValue = Math.Round(c.TotalSpent / c.OrderCount, 2),
                LastOrderDate = c.LastOrderDate,
                FirstOrderDate = c.FirstOrderDate,
                CustomerSegment = GetCustomerSegment(c.TotalSpent, c.OrderCount)
            }).ToList();
        }

        public async Task<List<ChartDataPointDto>> GetDailyRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var dailyData = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new ChartDataPointDto
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            return dailyData.Select(d => new ChartDataPointDto
            {
                Label = d.Label,
                Revenue = Math.Round(d.Revenue, 2),
                OrderCount = d.OrderCount,
                AverageOrderValue = Math.Round(d.AverageOrderValue, 2)
            }).ToList();
        }

        public async Task<List<ChartDataPointDto>> GetWeeklyRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .ToListAsync();

            var weeklyData = orders
                .GroupBy(o => GetWeekNumber(o.CreatedAt))
                .Select(g => new ChartDataPointDto
                {
                    Label = $"Week {g.Key}",
                    Revenue = Math.Round(g.Sum(o => o.TotalAmount), 2),
                    OrderCount = g.Count(),
                    AverageOrderValue = Math.Round(g.Average(o => o.TotalAmount), 2)
                })
                .OrderBy(x => x.Label)
                .ToList();

            return weeklyData;
        }

        public async Task<List<ChartDataPointDto>> GetMonthlyRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var monthlyData = await _context.Set<OrderEntity>()
                .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new ChartDataPointDto
                {
                    Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count(),
                    AverageOrderValue = g.Average(o => o.TotalAmount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            return monthlyData.Select(d => new ChartDataPointDto
            {
                Label = d.Label,
                Revenue = Math.Round(d.Revenue, 2),
                OrderCount = d.OrderCount,
                AverageOrderValue = Math.Round(d.AverageOrderValue, 2)
            }).ToList();
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count = 10)
        {
            var recentOrders = await _context.Set<OrderEntity>()
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNumber,
                    o.UserId,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt,
                    ItemCount = o.Items.Count
                })
                .ToListAsync();

            var userIds = recentOrders.Select(o => o.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Fullname })
                .ToDictionaryAsync(u => u.Id);

            return recentOrders.Select(o => new RecentOrderDto
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = users.ContainsKey(o.UserId) ? users[o.UserId].Fullname : "Unknown",
                TotalAmount = Math.Round(o.TotalAmount, 2),
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt,
                ItemCount = o.ItemCount
            }).ToList();
        }

        public async Task<ProductPerformanceDto> GetProductPerformanceAsync()
        {
            var totalProducts = await _context.Products.CountAsync();
            
            var variantStats = await _context.ProductVariants
                .GroupBy(v => 1)
                .Select(g => new
                {
                    OutOfStock = g.Count(v => v.Stock == 0),
                    LowStock = g.Count(v => v.Stock > 0 && v.Stock < 10)
                })
                .FirstOrDefaultAsync();

            var lowStockProducts = await _context.ProductVariants
                .Where(v => v.Stock > 0 && v.Stock < 10)
                .Include(v => v.Product)
                .Select(v => new LowStockProductDto
                {
                    ProductId = v.ProductId,
                    ProductName = v.Product.Name,
                    VariantId = v.Id,
                    VariantSku = v.Sku,
                    CurrentStock = v.Stock,
                    RecommendedStock = 20
                })
                .Take(10)
                .ToListAsync();

            var productsWithNoSales = await _context.Products
                .Where(p => p.TotalSold == 0)
                .CountAsync();

            var avgRating = await _context.Set<Domain.Review.ProductReview>()
                .Where(r => r.Status == Domain.Review.ReviewStatus.Approved)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            return new ProductPerformanceDto
            {
                TotalProducts = totalProducts,
                OutOfStockProducts = variantStats?.OutOfStock ?? 0,
                LowStockProducts = variantStats?.LowStock ?? 0,
                LowStockAlerts = lowStockProducts,
                ProductsWithNoSales = productsWithNoSales,
                AverageProductRating = Math.Round(avgRating, 2)
            };
        }

        private string GetCustomerSegment(decimal totalSpent, int orderCount)
        {
            if (totalSpent >= 10000000 || orderCount >= 10) // 10M VND or 10+ orders
                return "VIP";
            if (totalSpent >= 5000000 || orderCount >= 5) // 5M VND or 5+ orders
                return "Regular";
            return "New";
        }

        private int GetWeekNumber(DateTime date)
        {
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            return cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        // Convenience methods for "last N periods"
        public async Task<List<ChartDataPointDto>> GetLastNDaysRevenueAsync(int numberOfDays)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.Date.AddDays(-numberOfDays);
            return await GetDailyRevenueAsync(startDate, endDate);
        }

        public async Task<List<ChartDataPointDto>> GetLastNWeeksRevenueAsync(int numberOfWeeks)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.Date.AddDays(-numberOfWeeks * 7);
            return await GetWeeklyRevenueAsync(startDate, endDate);
        }

        public async Task<List<ChartDataPointDto>> GetLastNMonthsRevenueAsync(int numberOfMonths)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.Date.AddMonths(-numberOfMonths);
            return await GetMonthlyRevenueAsync(startDate, endDate);
        }
    }
}
