using AutoMapper;
using System.Runtime.InteropServices;
using Tekno.Api.Models.Catalog;
using Tekno.Api.Models.Catalog.Admin;
using Tekno.Application.Catalog.DTOs;

namespace Tekno.Api.Mappings
{
    public class CategoryApiProfile : Profile
    {
        public CategoryApiProfile()
        {
            // map cơ bản cho từng node
            CreateMap<CategoryTreeDto, CategoryTreeLandingDto>();

            // map list đệ quy
            CreateMap<CategoryDto, CategoryTreeLandingDto>();
        }
    }
    public class BrandApiProfile : Profile
    {
        public BrandApiProfile()
        {
            CreateMap<BrandDto, BrandApiDto>();
            CreateMap<BrandDto, CreateBrandApiDto>().ReverseMap();
            CreateMap<BrandDto, DeleteBrandApiDto>().ReverseMap();
            CreateMap<BrandDto, UpdateBrandApiDto>().ReverseMap();
        }
    }
}
