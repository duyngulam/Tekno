namespace Tekno.Application.Payment.Configuration
{
    /// <summary>
    /// VNPay gateway configuration settings
    /// This is a simple data model with no dependencies
    /// </summary>
    public class VNPaySettings
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string ReturnUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";

        /// <summary>
        /// Number of minutes the VNPay payment link should be valid (vnp_ExpireDate)
        /// Default is 15 minutes.
        /// </summary>
        public int ExpireMinutes { get; set; } = 15;
        
        /// <summary>
        /// Validate that required settings are configured
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TmnCode))
                throw new InvalidOperationException("VNPay TmnCode is not configured");
            
            if (string.IsNullOrWhiteSpace(HashSecret))
                throw new InvalidOperationException("VNPay HashSecret is not configured");
            
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                throw new InvalidOperationException("VNPay ReturnUrl is not configured");

            if (ExpireMinutes <= 0)
                throw new InvalidOperationException("VNPay ExpireMinutes must be a positive integer");
        }
    }
}
