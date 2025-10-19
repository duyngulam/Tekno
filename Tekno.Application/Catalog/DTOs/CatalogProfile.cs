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
}
