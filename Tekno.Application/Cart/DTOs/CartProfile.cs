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
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
