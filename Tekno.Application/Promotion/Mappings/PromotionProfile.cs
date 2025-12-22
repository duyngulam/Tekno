using AutoMapper;
using Tekno.Application.Promotion.DTOs;
using PromotionEntity = Tekno.Domain.Promotion.Promotion;

namespace Tekno.Application.Promotion.Mappings
{
    public class PromotionProfile : Profile
    {
        public PromotionProfile()
        {
            CreateMap<PromotionEntity, PromotionDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ApplicableCategoryIds, opt => opt.MapFrom(src => src.ApplicableCategories.Select(c => c.CategoryId).ToList()))
                .ForMember(dest => dest.ApplicableProductIds, opt => opt.MapFrom(src => src.ApplicableProducts.Select(p => p.ProductId).ToList()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.AffectedProductsCount, opt => opt.MapFrom(src => 
                    src.ApplicableProducts.Count + src.ApplicableCategories.Count * 10)); // Estimate
        }
    }
}
