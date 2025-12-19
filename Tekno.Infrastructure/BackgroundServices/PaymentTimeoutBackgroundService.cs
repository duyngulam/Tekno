using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tekno.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background service that periodically checks for timed-out payments
    /// Runs every 5 minutes by default
    /// </summary>
    public class PaymentTimeoutBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentTimeoutBackgroundService> _logger;
        private readonly TimeSpan _checkInterval;

        public PaymentTimeoutBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<PaymentTimeoutBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Default: Check every 5 minutes
            // Can be configured via environment variable
            var intervalMinutes = int.TryParse(
                Environment.GetEnvironmentVariable("PAYMENT_TIMEOUT_CHECK_INTERVAL_MINUTES"),
                out var minutes) ? minutes : 5;

            _checkInterval = TimeSpan.FromMinutes(intervalMinutes);

            _logger.LogInformation(
                "PaymentTimeoutBackgroundService initialized: Check interval = {Interval} minutes",
                intervalMinutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PaymentTimeoutBackgroundService started");

            // Wait 1 minute before first check (let application fully start)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckPaymentTimeoutsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in payment timeout background service");
                }

                // Wait for next check interval
                try
                {
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Expected when stopping the service
                    break;
                }
            }

            _logger.LogInformation("PaymentTimeoutBackgroundService stopped");
        }

        private async Task CheckPaymentTimeoutsAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting payment timeout check cycle");

                // Create a new scope for each check to ensure proper disposal
                using (var scope = _serviceProvider.CreateScope())
                {
                    var timeoutService = scope.ServiceProvider
                        .GetRequiredService<Application.Payment.Services.PaymentTimeoutService>();

                    await timeoutService.CheckTimeoutsAsync(cancellationToken);
                }

                _logger.LogInformation("Payment timeout check cycle completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check payment timeouts");
                // Don't throw - continue running
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PaymentTimeoutBackgroundService is stopping");
            await base.StopAsync(cancellationToken);
        }
    }
}
