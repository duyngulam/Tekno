using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog
{
    public class BrandApiDto
    {
        public int Id { get; private set; }
        [Required]
        public string Name { get; private set; } = string.Empty;
        [Required, RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug không hợp lệ")]
        public string Slug { get; private set; } = string.Empty;
        public string? Country { get; private set; }
        public string? LogoPath { get; private set; }
    }
}
