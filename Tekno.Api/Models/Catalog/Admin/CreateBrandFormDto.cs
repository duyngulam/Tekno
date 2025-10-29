using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog.Admin
{
    public class CreateBrandFormDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;
        public string? Country { get; set; }
        public IFormFile? LogoFile { get; set; }

    }
}
