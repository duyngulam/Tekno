using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Domain.Auth;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Auth.DTOs
{
    public class AuthProfile : Profile
    {
        public AuthProfile(){
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
                .ReverseMap();
        }
    }
}
