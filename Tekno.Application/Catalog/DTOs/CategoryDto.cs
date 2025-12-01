using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Catalog.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } // NEW: Main category image
        public int? ParentId { get; set; }
        public string? Description { get; set; }
    }
}
