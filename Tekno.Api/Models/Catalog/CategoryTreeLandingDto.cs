using System.ComponentModel;

namespace Tekno.Api.Models.Catalog
{
    public class CategoryTreeLandingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? IconPath { get; set; }
        public List<CategoryTreeLandingDto> SubCategories { get; set; } = new();
    }
}
