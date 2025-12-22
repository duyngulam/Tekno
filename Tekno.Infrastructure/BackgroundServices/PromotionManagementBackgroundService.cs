using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tekno.Application.Promotion.Interface;
using Tekno.Application.Promotion.Services;
using PromotionEntity = Tekno.Domain.Promotion.Promotion;

namespace Tekno.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background service that manages promotion lifecycle
    /// - Activates scheduled promotions when start time is reached
    /// - Expires active promotions when end time is reached
    /// - Automatically applies/removes discounts to products
    /// Runs every 30 minutes
    /// </summary>
    public class PromotionManagementBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PromotionManagementBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public PromotionManagementBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<PromotionManagementBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Promotion management background service started at {Time}", DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPromotionLifecycleAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing promotion lifecycle");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Promotion management background service stopped at {Time}", DateTime.UtcNow);
        }

        private async Task ProcessPromotionLifecycleAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var promotionRepository = scope.ServiceProvider.GetRequiredService<IPromotionRepository>();
            var promotionService = scope.ServiceProvider.GetRequiredService<PromotionService>();

            _logger.LogDebug("Checking promotion lifecycle at {Time}", DateTime.UtcNow);

            // 1. Activate scheduled promotions
            await ActivateScheduledPromotionsAsync(promotionRepository, promotionService);

            // 2. Expire active promotions
            await ExpireActivePromotionsAsync(promotionRepository, promotionService);
        }

        private async Task ActivateScheduledPromotionsAsync(
            IPromotionRepository promotionRepository,
            PromotionService promotionService)
        {
            var scheduledPromotions = await promotionRepository.GetScheduledPromotionsToActivateAsync();
            var scheduledList = scheduledPromotions.ToList();

            if (!scheduledList.Any())
            {
                _logger.LogDebug("No scheduled promotions to activate");
                return;
            }

            _logger.LogInformation("Found {Count} scheduled promotion(s) to activate", scheduledList.Count);

            foreach (var promotion in scheduledList)
            {
                try
                {
                    _logger.LogInformation(
                        "Activating promotion '{Name}' (ID: {Id})",
                        promotion.Name,
                        promotion.Id);

                    promotion.Activate();
                    await promotionRepository.UpdateAsync(promotion);

                    // Apply to products
                    await promotionService.ApplyPromotionToProductsAsync(promotion);

                    _logger.LogInformation(
                        "Successfully activated promotion '{Name}'",
                        promotion.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to activate promotion '{Name}' (ID: {Id})",
                        promotion.Name,
                        promotion.Id);
                }
            }
        }

        private async Task ExpireActivePromotionsAsync(
            IPromotionRepository promotionRepository,
            PromotionService promotionService)
        {
            var activePromotions = await promotionRepository.GetActivePromotionsToExpireAsync();
            var expiredList = activePromotions.ToList();

            if (!expiredList.Any())
            {
                _logger.LogDebug("No active promotions to expire");
                return;
            }

            _logger.LogInformation("Found {Count} active promotion(s) to expire", expiredList.Count);

            foreach (var promotion in expiredList)
            {
                try
                {
                    _logger.LogInformation(
                        "Expiring promotion '{Name}' (ID: {Id}). End date: {EndDate}",
                        promotion.Name,
                        promotion.Id,
                        promotion.EndDate);

                    promotion.MarkAsExpired();
                    await promotionRepository.UpdateAsync(promotion);

                    // Remove from products
                    await promotionService.RemovePromotionFromProductsAsync(promotion);

                    _logger.LogInformation(
                        "Successfully expired promotion '{Name}'",
                        promotion.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to expire promotion '{Name}' (ID: {Id})",
                        promotion.Name,
                        promotion.Id);
                }
            }
        }
    }
}
