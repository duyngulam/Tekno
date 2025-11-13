using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.DTOs
{
    public class BrandDto
    {
        public int Id { get;  set; }
        public string Name { get;  set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? LogoPath { get;  set; }
    }
}
