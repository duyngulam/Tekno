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
                throw new NotSupportedException($"Payment gateway '{gateway}' is not configured");
            }

            return implementation;
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
