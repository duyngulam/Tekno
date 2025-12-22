using AutoMapper;
using Tekno.Application.Payment.DTOs;
using Tekno.Domain.Payment;
using System.Linq;

namespace Tekno.Application.Payment.DTOs
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            // Payment -> PaymentStatusDto (without order details)
            CreateMap<Domain.Payment.Payment, PaymentStatusDto>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => 
                    src.Order != null ? src.Order.OrderNumber : string.Empty))
                .ForMember(dest => dest.GatewayName, opt => opt.MapFrom(src => GetGatewayName(src.Gateway)))
                .ForMember(dest => dest.MethodName, opt => opt.MapFrom(src => GetMethodName(src.Method)))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetStatusName(src.Status)))
                .ForMember(dest => dest.Order, opt => opt.Ignore()); // Don't auto-map, load manually when needed

            // Order -> OrderDetailsDto
            CreateMap<Domain.Order.Order, OrderDetailsDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetOrderStatusName(src.Status)))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // OrderItem -> OrderItemDetailDto
            CreateMap<Domain.Order.OrderItem, OrderItemDetailDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // Must be loaded from repository
                .ForMember(dest => dest.Variant, opt => opt.Ignore()); // Must be loaded from repository
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

        private static string GetOrderStatusName(Domain.Order.OrderStatus status)
        {
            return status switch
            {
                Domain.Order.OrderStatus.Pending => "Pending",
                Domain.Order.OrderStatus.Processing => "Processing",
                Domain.Order.OrderStatus.Completed => "Completed",
                Domain.Order.OrderStatus.Cancelled => "Cancelled",
                _ => status.ToString()
            };
        }
    }
}
