using AutoMapper;
using Tekno.Api.Models.Catalog;
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
}
