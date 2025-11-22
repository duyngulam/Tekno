using AutoMapper;
using System.Linq;
using Tekno.Application.Promotion.DTOs;
using Tekno.Domain.Promotion;

namespace Tekno.Application.Promotion.DTOs
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.RemainingQuantity, opt => opt.MapFrom(src => src.RemainingQuantity))
                .ForMember(dest => dest.ApplicableCategoryIds, opt => opt.MapFrom(src => 
                    src.ApplicableCategories.Select(c => c.CategoryId).ToList()))
                .ForMember(dest => dest.ApplicableProductIds, opt => opt.MapFrom(src => 
                    src.ApplicableProducts.Select(p => p.ProductId).ToList()));

            CreateMap<CouponUsage, CouponUsageDto>()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon.Code))
                .ForMember(dest => dest.CouponName, opt => opt.MapFrom(src => src.Coupon.Name));
        }
    }
}
