using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;

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
            CreateMap<Brand,BrandDto>().ReverseMap();
        }
    }
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.PrimaryImagePath,
                    opt => opt.MapFrom(src =>
                        src.Images.FirstOrDefault(i => i.IsPrimary) != null
                            ? src.Images.First(i => i.IsPrimary).ImageUrl
                            : "https://i.pinimg.com/736x/bd/e2/b8/bde2b888e9f57b2eee6f5ce3c90ce400.jpg"
                    ))
                .ForMember(dest => dest.DiscountPercent, opt => opt.Ignore());
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.Attributes, o => o.MapFrom(s =>
                    s.VariantAttributes.ToDictionary(
                        va => va.Attribute.Name,
                        va => va.Value.Value // ⚠️ Nếu Value là entity có property Value
                    )));
        }
    }
}
