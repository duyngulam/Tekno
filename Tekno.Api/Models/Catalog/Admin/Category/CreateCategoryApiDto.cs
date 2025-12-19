using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog.Admin.Category
{
    public class CreateCategoryApiDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug không hợp lệ")]
        public string Slug { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public string? Description { get; set; }

        public IFormFile? IconFile { get; set; }

        public IFormFile? ImageFile { get; set; } // NEW: Main category image
    }
}
