using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tekno.Application.Promotion.Interface;

namespace Tekno.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background service that automatically marks expired coupons and product discounts as expired
    /// Runs every hour to check and update statuses
    /// </summary>
    public class CouponExpirationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CouponExpirationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public CouponExpirationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CouponExpirationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Coupon expiration background service started at {Time}", DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredCouponsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing expired coupons");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Coupon expiration background service stopped at {Time}", DateTime.UtcNow);
        }

        private async Task ProcessExpiredCouponsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var couponRepository = scope.ServiceProvider.GetRequiredService<ICouponRepository>();

            _logger.LogDebug("Checking for expired coupons at {Time}", DateTime.UtcNow);

            // Get all active coupons that have expired
            var expiredCoupons = await couponRepository.GetExpiredActiveCouponsAsync();
            var expiredList = expiredCoupons.ToList();

            if (!expiredList.Any())
            {
                _logger.LogDebug("No expired coupons found");
                return;
            }

            _logger.LogInformation("Found {Count} expired coupon(s) to process", expiredList.Count);

            var processedCount = 0;
            foreach (var coupon in expiredList)
            {
                try
                {
                    coupon.MarkAsExpired();
                    await couponRepository.UpdateAsync(coupon);
                    processedCount++;
                    
                    _logger.LogInformation(
                        "Marked coupon '{Code}' ({Name}) as expired. End date: {EndDate}, Used: {Used}/{Total}",
                        coupon.Code, 
                        coupon.Name, 
                        coupon.EndDate, 
                        coupon.UsedCount, 
                        coupon.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Failed to mark coupon '{Code}' as expired", 
                        coupon.Code);
                }
            }

            _logger.LogInformation(
                "Successfully processed {Processed} out of {Total} expired coupons", 
                processedCount, 
                expiredList.Count);
        }
    }
}
