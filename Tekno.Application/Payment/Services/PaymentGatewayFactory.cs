using System;
using System.Collections.Generic;
using System.Linq;
using Tekno.Application.Payment.Interfaces;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.Services
{
    /// <summary>
    /// Factory to get payment gateway implementations
    /// Makes it easy to add/switch payment gateways
    /// </summary>
    public class PaymentGatewayFactory
    {
        private readonly IEnumerable<IPaymentGateway> _gateways;

        public PaymentGatewayFactory(IEnumerable<IPaymentGateway> gateways)
        {
            _gateways = gateways;
        }

        /// <summary>
        /// Get payment gateway by type
        /// </summary>
        public IPaymentGateway GetGateway(PaymentGateway gateway)
        {
            var implementation = _gateways.FirstOrDefault(g => g.Gateway == gateway);
            
            if (implementation == null)
            {
                var availableGateways = string.Join(", ", _gateways.Select(g => $"{g.Gateway} ({(int)g.Gateway})"));
                var message = $"Payment gateway '{gateway}' ({(int)gateway}) is not implemented or configured. " +
                             $"Available gateways: {availableGateways}";
                throw new NotSupportedException(message);
            }

            return implementation;
        }

        /// <summary>
        /// Check if gateway is available
        /// </summary>
        public bool IsGatewayAvailable(PaymentGateway gateway)
        {
            return _gateways.Any(g => g.Gateway == gateway);
        }

        /// <summary>
        /// Get all available gateways
        /// </summary>
        public IEnumerable<PaymentGateway> GetAvailableGateways()
        {
            return _gateways.Select(g => g.Gateway);
        }
    }
}
