using AutoMapper;
using System.Text.Json;
using Tekno.Domain.Catalog;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Application.Catalog.DTOs
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryDto, CategoryTreeDto>().ReverseMap();
        }
    }
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDto>().ReverseMap();
        }
    }
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            // ===== Summary =====
            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.PrimaryImagePath, opt => opt.MapFrom(src =>
                    src.Images.FirstOrDefault(i => i.IsPrimary) != null
                        ? src.Images.First(i => i.IsPrimary).ImageUrl
                        : "https://i.pinimg.com/736x/bd/e2/b8/bde2b888e9f57b2eee6f5ce3c90ce400.jpg"))
                .ForMember(dest => dest.DiscountPercent, opt => opt.Ignore());

            // ===== Variant =====
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.Attributes, o => o.MapFrom(s =>
                    s.VariantAttributes.ToDictionary(
                        va => va.Attribute.Name,
                        va => va.Value.Value 
                    )));

            // ===== Detail =====
            CreateMap<Product, ProductDetailDto>()
                        .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                        .ForMember(dest => dest.Specs, opt => opt.MapFrom(src => src.Detail.Specs))
                        .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)))
                        .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));

            CreateMap<ProductVariantAttribute, ProductAttributeDto>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Attribute.Name))
           .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value.Value));
        }
    }
}
