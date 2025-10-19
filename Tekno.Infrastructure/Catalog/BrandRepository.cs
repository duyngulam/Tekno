using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.Interface;
using Tekno.Domain.Catalog;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Catalog
{
    public class BrandRepository : IBrandRepository
    {
        private readonly AppDbContext _context;
        public BrandRepository (AppDbContext context)
        {
            _context = context;
        }
        public async Task <List<Brand?>> GetAllBrandsAsync()
        {
            return await _context.Brands.AsNoTracking().ToListAsync();
        }
        public async Task <Brand?> GetBrandBySlugAsync(string slug)
        {
            return await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Slug == slug);
        }
    }
}
