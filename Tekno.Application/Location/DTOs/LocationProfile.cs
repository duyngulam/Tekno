using AutoMapper;
using Tekno.Application.Location.DTOs;
using Tekno.Domain.Location;

namespace Tekno.Application.Location.DTOs
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<Province, ProvinceDto>();
            CreateMap<District, DistrictDto>();
            CreateMap<Ward, WardDto>();
        }
    }
}
