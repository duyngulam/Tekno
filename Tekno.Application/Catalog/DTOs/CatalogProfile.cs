using AutoMapper;
using System.Text.Json;
using Tekno.Domain.Catalog;
using Tekno.Application.Catalog.DTOs;

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
                        va => va.Value.Value // ✅ đúng với domain bạn có
                    )));

            // ===== Detail =====
            CreateMap<Product, ProductDetailDto>()
    .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand.Name))
    .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
    .ForMember(d => d.Specs, o => o.Ignore()) // tạm bỏ qua Specs
    .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(i => i.ImageUrl)))
    .ForMember(d => d.Variants, o => o.MapFrom(s => s.Variants))
    .AfterMap((src, dest) =>
    {
        dest.Specs = string.IsNullOrWhiteSpace(src.Detail?.Specs)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(src.Detail.Specs)!;
    });

        }
    }
}
