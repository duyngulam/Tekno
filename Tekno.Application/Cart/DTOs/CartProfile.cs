using AutoMapper;
using System.Linq;
using Tekno.Domain.Cart;

namespace Tekno.Application.Cart.DTOs
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            // UserCart -> CartDto
            CreateMap<UserCart, CartDto>();

            // CartItem -> CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                    src.Variant != null ? src.Variant.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => 
                    src.Variant != null ? src.Variant.Product.Slug : string.Empty))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => 
                    src.Variant != null ? src.Variant.Sku : string.Empty))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => 
                    src.Variant != null && src.Variant.Product.Brand != null ? src.Variant.Product.Brand.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => 
                    src.Variant != null && src.Variant.Product.Category != null ? src.Variant.Product.Category.Name : string.Empty))
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => 
                    src.Variant != null && src.Variant.Product.Images != null 
                        ? src.Variant.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                            ? src.Variant.Product.Images.First(i => i.IsPrimary).ImageUrl
                            : src.Variant.Product.Images.FirstOrDefault() != null
                                ? src.Variant.Product.Images.First().ImageUrl
                                : null
                        : null))
                .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => 
                    src.Variant != null ? src.Variant.Stock : 0))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => 
                    src.Variant != null ? src.Variant.VariantAttributes.Select(va => new VariantAttributeInfo
                    {
                        Name = va.Attribute != null ? va.Attribute.Name : string.Empty,
                        Value = va.Value != null ? va.Value.Value : string.Empty
                    }).ToList() : new System.Collections.Generic.List<VariantAttributeInfo>()));

            // Wishlist -> WishlistDto
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                    src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => 
                    src.Product != null ? src.Product.Slug : string.Empty))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => 
                    src.Product != null && src.Product.Brand != null ? src.Product.Brand.Name : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => 
                    src.Product != null && src.Product.Category != null ? src.Product.Category.Name : string.Empty))
                .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => 
                    src.Product != null ? src.Product.BasePrice : 0))
                .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => 
                    src.Product != null && src.Product.Images != null 
                        ? src.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                            ? src.Product.Images.First(i => i.IsPrimary).ImageUrl
                            : src.Product.Images.FirstOrDefault() != null
                                ? src.Product.Images.First().ImageUrl
                                : null
                        : null))
                .ForMember(dest => dest.TotalVariants, opt => opt.MapFrom(src => 
                    src.Product != null && src.Product.Variants != null ? src.Product.Variants.Count : 0))
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => 
                    src.Product != null && src.Product.Variants != null 
                        ? src.Product.Variants.Any(v => v.Stock > 0) 
                        : false));
        }
    }
}
