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
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Slug { get; private set; } = string.Empty;
        public string? Country { get; private set; }
        public string? LogoPath { get; private set; }
    }
}
