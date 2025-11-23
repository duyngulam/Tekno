using AutoMapper;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using Tekno.Domain.Catalog;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.DTOs.Admin;

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
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.DiscountPercent))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // ===== ProductVariant =====
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock));

            // ===== ProductVariantDetailDto =====
            CreateMap<ProductVariant, ProductVariantDetailDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand != null ? src.Product.Brand.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category != null ? src.Product.Category.Name : string.Empty))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes));

            // ===== ProductVariantAttribute → VariantAttributeDto =====
            CreateMap<ProductVariantAttribute, VariantAttributeDto>()
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute != null ? src.Attribute.Name : string.Empty))
                .ForMember(dest => dest.AttributeValue, opt => opt.MapFrom(src => src.Value != null ? src.Value.Value : string.Empty));

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
                {
                    if (string.IsNullOrWhiteSpace(src.Specs))
                    {
                        // KẾT QUẢ 3: Trả về logic fallback
                        return BuildSpecsFromVariants(src);
                    }

                    try
                    {
                        // KẾT QUẢ 1 & 2: Cố gắng Deserialize và xử lý null
                        return JsonSerializer.Deserialize<List<ProductAttributeDto>>(src.Specs)
                               ?? new List<ProductAttributeDto>();
                    }
                    catch (JsonException)
                    {
                        return new List<ProductAttributeDto>();
                    }
                }));
            // ===== CreateProduct =====
            // Map from domain to DTO (ignore Images mapping)
            CreateMap<Product, CreateProductDto>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            // Map from DTO to domain. Images (IFormFile) must be uploaded and converted to ProductImage separately.
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // ===== ProductAttribute -> ProductAttributeDto =====
            CreateMap<ProductAttribute, ProductAttributeDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Values.Select(v => v.Value).ToList()));
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
