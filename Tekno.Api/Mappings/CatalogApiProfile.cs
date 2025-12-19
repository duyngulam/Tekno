using AutoMapper;
using System.Runtime.InteropServices;
using Tekno.Api.Models.Catalog;
using Tekno.Api.Models.Catalog.Admin.brand;
using Tekno.Api.Models.Catalog.Admin.Category;
using Tekno.Application.Catalog.DTOs;

namespace Tekno.Api.Mappings
{
    public class CategoryApiProfile : Profile
    {
        public CategoryApiProfile()
        {

            // map list đệ quy
            CreateMap<CategoryDto, CategoryTreeDto>();
            // admin mappings
            CreateMap<CategoryDto, CreateCategoryApiDto>().ReverseMap();
            CreateMap<CategoryDto, UpdateCategoryApiDto>().ReverseMap();
            CreateMap<CategoryDto, DeleteCategoryApiDto>().ReverseMap();
        }
    }
    public class BrandApiProfile : Profile
    {
        public BrandApiProfile()
        {
            CreateMap<BrandDto, CreateBrandApiDto>().ReverseMap();
            CreateMap<BrandDto, DeleteBrandApiDto>().ReverseMap();
            CreateMap<BrandDto, UpdateBrandApiDto>().ReverseMap();
        }
    }
}
