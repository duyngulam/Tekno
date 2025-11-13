using AutoMapper;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
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
            // ===== Product =====
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.PrimaryImagePath, opt => opt.MapFrom(src =>
                    src.Images.FirstOrDefault(i => i.IsPrimary) != null
                        ? src.Images.First(i => i.IsPrimary).ImageUrl
                        : "https://i.pinimg.com/736x/bd/e2/b8/bde2b888e9f57b2eee6f5ce3c90ce400.jpg"))
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.DiscountPercent));

            // ===== ProductVariant =====
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock));

            // ===== ProductVariantAttribute → ProductAttributeDto =====
            // Map the selected value (the variant's actual value), not all possible attribute values.
            CreateMap<ProductVariantAttribute, ProductAttributeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Attribute != null ? src.Attribute.Name : string.Empty))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src =>
                    src.Value != null
                        ? new List<string> { src.Value.Value ?? string.Empty }
                        : new List<string>()));

            // ===== ProductDetail =====
            CreateMap<Product, ProductDetailDto>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants))
                .ForMember(dest => dest.Specs, opt => opt.MapFrom((src, dest) =>
                    (src.Detail != null && !string.IsNullOrWhiteSpace(src.Detail.Specs))
                        ? JsonSerializer.Deserialize<List<ProductAttributeDto>>(src.Detail.Specs)
                        ?? new List<ProductAttributeDto>()
                        : BuildSpecsFromVariants(src)
                ));
        }

        private static List<ProductAttributeDto> BuildSpecsFromVariants(Product src)
        {
            var result = new Dictionary<string, HashSet<string>>(System.StringComparer.OrdinalIgnoreCase);

            if (src?.Variants == null) return new List<ProductAttributeDto>();

            foreach (var variant in src.Variants)
            {
                if (variant?.VariantAttributes == null) continue;
                foreach (var va in variant.VariantAttributes)
                {
                    var attrName = va?.Attribute?.Name?.Trim();
                    var value = va?.Value?.Value?.Trim();

                    if (string.IsNullOrEmpty(attrName) || string.IsNullOrEmpty(value)) continue;

                    var key = attrName;
                    if (!result.TryGetValue(key, out var set))
                    {
                        set = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
                        result[key] = set;
                    }

                    set.Add(value);
                }
            }

            return result.Select(kv => new ProductAttributeDto
            {
                Name = kv.Key,
                Value = kv.Value.Select(v => v).ToList()
            }).ToList();
        }
    }

}
