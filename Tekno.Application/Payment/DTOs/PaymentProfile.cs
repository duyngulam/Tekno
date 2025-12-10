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
                    src.Order != null ? src.Order.OrderNumber : string.Empty));

            // Can add reverse mappings if needed
        }
    }
}
