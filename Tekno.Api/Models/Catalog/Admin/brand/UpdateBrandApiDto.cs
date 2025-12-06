using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tekno.Api.Models.Catalog.Admin.brand
{
    public class UpdateBrandApiDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required, RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug không hợp lệ")]
        public string Slug { get; set; } = string.Empty;

        public string? Country { get; set; }
        public string? LogoPath { get; set; }

        // Logo file for upload (optional - only provide if changing logo)
        public IFormFile? LogoFile { get; set; }
    }
}
