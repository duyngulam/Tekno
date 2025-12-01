using AutoMapper;
using System.Linq;
using Tekno.Domain.Blog;

namespace Tekno.Application.Blog.DTOs
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            // BlogPost to BlogPostSummaryDto
            CreateMap<BlogPost, BlogPostSummaryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag).ToList()));

            // BlogPost to BlogPostDetailDto (without RelatedProducts - handled separately due to async)
            CreateMap<BlogPost, BlogPostDetailDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags.Select(t => t.Tag).ToList()))
                .ForMember(dest => dest.RelatedProducts, opt => opt.Ignore()); // Will be populated manually
        }
    }
}
