using Microsoft.Extensions.Configuration;
using Tekno.Application.Payment.Configuration;

namespace Tekno.Infrastructure.Payment.Configuration
{
    /// <summary>
    /// VNPay settings provider - Infrastructure layer
    /// Responsible for loading configuration from external sources
    /// </summary>
    public class VNPaySettingsProvider
    {
        /// <summary>
        /// Load VNPay settings from configuration sources
        /// Priority: Environment Variables > appsettings.json
        /// </summary>
        public static VNPaySettings LoadSettings(IConfiguration configuration)
        {
            var settings = new VNPaySettings
            {
                // Priority 1: Environment variables (for production/Docker)
                // Priority 2: appsettings.json (for development)
                TmnCode = Environment.GetEnvironmentVariable("VNPAY_TMN_CODE")
                         ?? configuration["VNPay:TmnCode"]
                         ?? string.Empty,

                HashSecret = Environment.GetEnvironmentVariable("VNPAY_HASH_SECRET")
                            ?? configuration["VNPay:HashSecret"]
                            ?? string.Empty,

                PaymentUrl = Environment.GetEnvironmentVariable("VNPAY_PAYMENT_URL")
                            ?? configuration["VNPay:PaymentUrl"]
                            ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",

                ReturnUrl = Environment.GetEnvironmentVariable("VNPAY_RETURN_URL")
                           ?? configuration["VNPay:ReturnUrl"]
                           ?? string.Empty,

                ApiUrl = Environment.GetEnvironmentVariable("VNPAY_API_URL")
                        ?? configuration["VNPay:ApiUrl"]
                        ?? "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction"
            };

            return settings;
        }
    }
}
