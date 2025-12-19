using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Payment.Configuration;
using Tekno.Application.Payment.Interfaces;
using Tekno.Application.Payment.Models;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Gateways
{
    /// <summary>
    /// VNPay payment gateway implementation for Vietnam
    /// Documentation: https://sandbox.vnpayment.vn/apis/docs/gioi-thieu/
    /// </summary>
    public class VNPayPaymentGateway : IPaymentGateway
    {
        private readonly VNPaySettings _settings;
        private readonly IAppLogger<VNPayPaymentGateway> _logger;
        private readonly string _version = "2.1.0";
        private readonly string _command = "pay";

        public PaymentGateway Gateway => PaymentGateway.VNPay;

        public VNPayPaymentGateway(
            VNPaySettings settings,
            IAppLogger<VNPayPaymentGateway> logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;

            // Validate settings on construction
            _settings.Validate();

            _logger.LogInformation("VNPay gateway initialized with TmnCode: {TmnCode}", _settings.TmnCode);
        }

        public async Task<PaymentInitResult> InitiatePaymentAsync(PaymentRequest request)
        {
            try
            {
                _logger.LogInformation("VNPay payment initiated for order {OrderNumber}, amount {Amount:N0} {Currency}",
                    request.OrderNumber, request.Amount, request.Currency);

                // VNPay requires amount * 100 (no decimal point)
                var vnpAmount = ((long)(request.Amount * 100)).ToString();

                // Create date in VNPay format: yyyyMMddHHmmss (GMT+7)
                var vnpCreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                
                // Expire date (15 minutes from now)
                var vnpExpireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

                // Get client IP (use request IP or default)
                var vnpIpAddr = request.IpAddress ?? "127.0.0.1";

                // Build payment parameters
                var vnpParams = new SortedDictionary<string, string>
                {
                    { "vnp_Version", _version },
                    { "vnp_Command", _command },
                    { "vnp_TmnCode", _settings.TmnCode },
                    { "vnp_Amount", vnpAmount },
                    { "vnp_CurrCode", "VND" }, // VNPay only supports VND
                    { "vnp_TxnRef", request.OrderNumber }, // Use order number as transaction reference
                    { "vnp_OrderInfo", SanitizeOrderInfo(request.OrderNumber, request.Amount) },
                    { "vnp_OrderType", GetOrderType(request.Method) },
                    { "vnp_Locale", "vn" }, // Vietnamese language
                    { "vnp_ReturnUrl", request.ReturnUrl },
                    { "vnp_IpAddr", vnpIpAddr },
                    { "vnp_CreateDate", vnpCreateDate },
                    { "vnp_ExpireDate", vnpExpireDate }
                };

                // Add bank code if specified
                var bankCode = GetBankCode(request.Method);
                if (!string.IsNullOrEmpty(bankCode))
                {
                    vnpParams.Add("vnp_BankCode", bankCode);
                }

                // Generate secure hash (HMACSHA512)
                var secureHash = GenerateSecureHash(vnpParams, _settings.HashSecret);
                vnpParams.Add("vnp_SecureHash", secureHash);

                // Build payment URL
                var paymentUrl = BuildPaymentUrl(_settings.PaymentUrl, vnpParams);

                _logger.LogInformation("VNPay payment URL generated for order {OrderNumber}", request.OrderNumber);

                return await Task.FromResult(new PaymentInitResult
                {
                    Success = true,
                    TransactionId = request.OrderNumber, // VNPay uses TxnRef as transaction ID
                    PaymentUrl = paymentUrl,
                    GatewayResponse = new
                    {
                        message = "VNPay payment URL generated successfully",
                        tmnCode = _settings.TmnCode,
                        txnRef = request.OrderNumber,
                        amount = request.Amount,
                        amountVnp = vnpAmount,
                        currency = "VND",
                        createDate = vnpCreateDate,
                        expireDate = vnpExpireDate,
                        locale = "vn",
                        orderInfo = vnpParams["vnp_OrderInfo"],
                        paymentUrl = paymentUrl,
                        note = "Redirect customer to paymentUrl to complete payment. Payment expires in 15 minutes."
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VNPay payment initiation failed for order {OrderNumber}", request.OrderNumber);
                return new PaymentInitResult
                {
                    Success = false,
                    ErrorMessage = $"VNPay payment initiation failed: {ex.Message}"
                };
            }
        }

        public Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId, object callbackData)
        {
            try
            {
                _logger.LogInformation("VNPay payment verification for transaction {TransactionId}", transactionId);

                // Parse callback data from VNPay
                var vnpData = ParseVNPayCallback(callbackData);

                if (vnpData == null || !vnpData.Any())
                {
                    _logger.LogWarning("VNPay callback data is empty or invalid");
                    return Task.FromResult(new PaymentVerificationResult
                    {
                        IsValid = false,
                        IsSuccessful = false,
                        TransactionId = transactionId,
                        ErrorMessage = "Invalid callback data from VNPay",
                        GatewayResponse = callbackData
                    });
                }

                // Extract parameters
                var vnpSecureHash = vnpData.ContainsKey("vnp_SecureHash") ? vnpData["vnp_SecureHash"] : "";
                var vnpResponseCode = vnpData.ContainsKey("vnp_ResponseCode") ? vnpData["vnp_ResponseCode"] : "";
                var vnpTransactionStatus = vnpData.ContainsKey("vnp_TransactionStatus") ? vnpData["vnp_TransactionStatus"] : "";
                var vnpTxnRef = vnpData.ContainsKey("vnp_TxnRef") ? vnpData["vnp_TxnRef"] : "";
                var vnpAmount = vnpData.ContainsKey("vnp_Amount") ? vnpData["vnp_Amount"] : "0";
                var vnpOrderInfo = vnpData.ContainsKey("vnp_OrderInfo") ? vnpData["vnp_OrderInfo"] : "";
                var vnpTransactionNo = vnpData.ContainsKey("vnp_TransactionNo") ? vnpData["vnp_TransactionNo"] : "";
                var vnpBankCode = vnpData.ContainsKey("vnp_BankCode") ? vnpData["vnp_BankCode"] : "";
                var vnpPayDate = vnpData.ContainsKey("vnp_PayDate") ? vnpData["vnp_PayDate"] : "";

                // Verify secure hash
                var paramsToVerify = vnpData
                    .Where(p => p.Key != "vnp_SecureHash" && p.Key != "vnp_SecureHashType")
                    .OrderBy(p => p.Key)
                    .ToDictionary(p => p.Key, p => p.Value);

                var computedHash = GenerateSecureHash(new SortedDictionary<string, string>(paramsToVerify), _settings.HashSecret);
                var isValidSignature = computedHash.Equals(vnpSecureHash, StringComparison.OrdinalIgnoreCase);

                if (!isValidSignature)
                {
                    _logger.LogWarning("VNPay signature verification failed for transaction {TransactionId}", transactionId);
                    return Task.FromResult(new PaymentVerificationResult
                    {
                        IsValid = false,
                        IsSuccessful = false,
                        TransactionId = transactionId,
                        ErrorMessage = "Invalid signature from VNPay - possible tampering detected",
                        GatewayResponse = callbackData
                    });
                }

                // Check response code
                // 00 = Success, others = Failed
                var isSuccess = vnpResponseCode == "00" && vnpTransactionStatus == "00";

                // Parse amount (VNPay sends amount * 100)
                decimal amount = 0;
                if (decimal.TryParse(vnpAmount, out var vnpAmountDecimal))
                {
                    amount = vnpAmountDecimal / 100; // Divide by 100 to get actual amount
                }

                var result = new PaymentVerificationResult
                {
                    IsValid = true,
                    IsSuccessful = isSuccess,
                    TransactionId = vnpTxnRef, // Use TxnRef as transaction ID
                    Amount = amount,
                    Currency = "VND",
                    ErrorMessage = isSuccess ? null : GetVNPayErrorMessage(vnpResponseCode),
                    GatewayResponse = new
                    {
                        message = "VNPay verification completed",
                        transactionId = vnpTxnRef,
                        vnpTransactionNo = vnpTransactionNo,
                        responseCode = vnpResponseCode,
                        transactionStatus = vnpTransactionStatus,
                        amount = amount,
                        amountVnp = vnpAmount,
                        currency = "VND",
                        bankCode = vnpBankCode,
                        orderInfo = vnpOrderInfo,
                        payDate = vnpPayDate,
                        signatureValid = true,
                        success = isSuccess
                    }
                };

                _logger.LogInformation("VNPay verification result: IsValid={IsValid}, IsSuccessful={IsSuccessful}, Amount={Amount} VND",
                    result.IsValid, result.IsSuccessful, result.Amount);

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VNPay verification failed for transaction {TransactionId}", transactionId);
                return Task.FromResult(new PaymentVerificationResult
                {
                    IsValid = false,
                    IsSuccessful = false,
                    TransactionId = transactionId,
                    ErrorMessage = $"VNPay verification error: {ex.Message}",
                    GatewayResponse = callbackData
                });
            }
        }

        public Task<RefundResult> RefundPaymentAsync(string transactionId, decimal amount, string reason)
        {
            // VNPay refund requires calling their merchant API
            // This would require additional implementation
            _logger.LogWarning("VNPay refund not implemented yet for transaction {TransactionId}", transactionId);

            return Task.FromResult(new RefundResult
            {
                Success = false,
                ErrorMessage = "VNPay refund feature not implemented yet. Please use VNPay merchant portal for refunds."
            });
        }

        #region Helper Methods

        /// <summary>
        /// Generate HMAC SHA512 secure hash for VNPay
        /// IMPORTANT: VNPay expects hash from RAW values, NOT URL-encoded values
        /// </summary>
        private string GenerateSecureHash(SortedDictionary<string, string> parameters, string secretKey)
        {
            // Build query string with RAW values (no URL encoding for hash calculation)
            // VNPay documentation: hash is calculated from raw parameter values
            var data = string.Join("&", parameters
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => $"{p.Key}={p.Value}")); // ← NO Uri.EscapeDataString here!

            _logger.LogInformation("VNPay hash input data: {Data}", data);

            // Compute HMAC SHA512
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();
                
                _logger.LogInformation("VNPay generated hash: {Hash}", hashString);
                
                return hashString;
            }
        }

        /// <summary>
        /// Build payment URL with parameters
        /// VNPay expects specific encoding: spaces as '+' not '%20'
        /// </summary>
        private string BuildPaymentUrl(string baseUrl, SortedDictionary<string, string> parameters)
        {
            var queryString = string.Join("&", parameters
                .Select(p => $"{p.Key}={UrlEncodeLikeVNPay(p.Value)}"));

            return $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// URL encode like VNPay expects: spaces as '+' instead of '%20'
        /// This matches VNPay's encoding style
        /// </summary>
        private string UrlEncodeLikeVNPay(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // First, URL encode normally
            var encoded = Uri.EscapeDataString(value);
            
            // Then replace %20 with + to match VNPay's format
            // VNPay uses application/x-www-form-urlencoded encoding where spaces = +
            encoded = encoded.Replace("%20", "+");
            
            return encoded;
        }

        /// <summary>
        /// Sanitize order info (VNPay requires Vietnamese without diacritics)
        /// IMPORTANT: Remove number formatting (commas, dots) as VNPay doesn't accept them
        /// </summary>
        private string SanitizeOrderInfo(string orderNumber, decimal amount)
        {
            // Format amount WITHOUT thousand separators (VNPay doesn't accept commas in order info)
            var info = $"Thanh toan don hang {orderNumber}. So tien {amount:F0} VND"; // F0 = no decimals, no commas
            
            // Remove diacritics (simplified - use a proper library for production)
            info = info.Replace("đ", "d").Replace("Đ", "D");
            info = info.Replace("ơ", "o").Replace("Ơ", "O");
            info = info.Replace("ư", "u").Replace("Ư", "U");
            
            // Remove special characters except space, numbers, and letters
            var sanitized = new string(info.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
            
            // Limit length to 255 characters
            if (sanitized.Length > 255)
                sanitized = sanitized.Substring(0, 255);

            _logger.LogInformation("VNPay OrderInfo (sanitized): {OrderInfo}", sanitized);

            return sanitized;
        }

        /// <summary>
        /// Get VNPay order type based on payment method
        /// </summary>
        private string GetOrderType(PaymentMethod method)
        {
            // VNPay order types: billpayment, fashion, other, etc.
            // Using "other" as default for e-commerce
            return "other";
        }

        /// <summary>
        /// Get VNPay bank code based on payment method
        /// </summary>
        private string? GetBankCode(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.EWallet => "VNPAYQR", // QR Code payment
                PaymentMethod.BankTransfer => "VNBANK", // ATM/Bank account
                PaymentMethod.CreditCard => "INTCARD", // International card
                PaymentMethod.DebitCard => "VNBANK", // ATM/Debit card
                _ => null // Let user choose at VNPay
            };
        }

        /// <summary>
        /// Parse VNPay callback data from various formats
        /// </summary>
        private Dictionary<string, string> ParseVNPayCallback(object callbackData)
        {
            var result = new Dictionary<string, string>();

            if (callbackData == null)
                return result;

            // If it's already a dictionary
            if (callbackData is Dictionary<string, string> dict)
            {
                return dict;
            }

            // If it's a query string or URL
            if (callbackData is string queryString)
            {
                result = ParseQueryString(queryString);
                return result;
            }

            // Try to serialize and deserialize as JSON
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(callbackData);
                var parsed = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (parsed != null)
                    return parsed;
            }
            catch
            {
                _logger.LogWarning("Failed to parse VNPay callback data");
            }

            return result;
        }

        /// <summary>
        /// Parse query string into dictionary (.NET Core compatible)
        /// </summary>
        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(queryString))
                return result;

            // Remove leading ? if present
            queryString = queryString.TrimStart('?');

            // Split by & and parse each key=value pair
            var pairs = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=', 2);
                if (keyValue.Length == 2)
                {
                    var key = Uri.UnescapeDataString(keyValue[0]);
                    var value = Uri.UnescapeDataString(keyValue[1]);
                    result[key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Get error message for VNPay response code
        /// </summary>
        private string GetVNPayErrorMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP)",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày",
                "75" => "Ngân hàng thanh toán đang bảo trì",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định",
                "99" => "Lỗi không xác định",
                _ => $"Lỗi không xác định (Mã lỗi: {responseCode})"
            };
        }

        #endregion
    }
}
