using AutoMapper;
using Tekno.Application.Payment.DTOs;
using Tekno.Domain.Payment;

namespace Tekno.Application.Payment.DTOs
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            // Payment -> PaymentStatusDto
            CreateMap<Domain.Payment.Payment, PaymentStatusDto>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => 
                    src.Order != null ? src.Order.OrderNumber : string.Empty))
                .ForMember(dest => dest.GatewayName, opt => opt.MapFrom(src => GetGatewayName(src.Gateway)))
                .ForMember(dest => dest.MethodName, opt => opt.MapFrom(src => GetMethodName(src.Method)))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetStatusName(src.Status)));
        }

        private static string GetGatewayName(PaymentGateway gateway)
        {
            return gateway switch
            {
                PaymentGateway.Mock => "Mock (Test)",
                PaymentGateway.Stripe => "Stripe",
                PaymentGateway.PayPal => "PayPal",
                PaymentGateway.VNPay => "VNPay",
                PaymentGateway.MoMo => "MoMo",
                PaymentGateway.ZaloPay => "ZaloPay",
                _ => gateway.ToString()
            };
        }

        private static string GetMethodName(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CreditCard => "Credit Card",
                PaymentMethod.DebitCard => "Debit Card",
                PaymentMethod.BankTransfer => "Bank Transfer",
                PaymentMethod.EWallet => "E-Wallet",
                PaymentMethod.Cash => "Cash (COD)",
                _ => method.ToString()
            };
        }

        private static string GetStatusName(PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Pending => "Pending",
                PaymentStatus.Processing => "Processing",
                PaymentStatus.Completed => "Completed",
                PaymentStatus.Failed => "Failed",
                PaymentStatus.Refunded => "Refunded",
                PaymentStatus.Cancelled => "Cancelled",
                _ => status.ToString()
            };
        }
    }
}
