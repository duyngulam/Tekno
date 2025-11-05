using System.ComponentModel.DataAnnotations;

namespace Tekno.Api.Models.Catalog.Admin
{
    public class UpdateCategoryApiDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug không hợp lệ")]
        public string Slug { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public IFormFile? IconFile { get; set; }
    }
}