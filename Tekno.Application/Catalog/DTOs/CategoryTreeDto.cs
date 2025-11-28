using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Catalog.DTOs
{
    public class CategoryTreeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? IconPath { get; set; }
        public string? ImageUrl { get; set; } // NEW: Main category image

        public List<CategoryTreeDto> SubCategories { get; set; } = new();
    }
}
