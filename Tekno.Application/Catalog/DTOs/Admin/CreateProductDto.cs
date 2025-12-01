using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Application.Catalog.DTOs.Admin
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Slug { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int BrandId { get; set; }

        public string? Status { get;  set; } = "available";
        public decimal BasePrice { get;  set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? WarrantyInfo { get; set; }
        public string? Overview { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public decimal? DiscountPercent { get; set; }
    }
}
