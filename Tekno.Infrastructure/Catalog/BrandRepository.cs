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
        public async Task<Brand?> GetBrandByIdAsync(int id)
        {
            return await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<Brand> CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> UpdateAsync(Brand brand)
        {
            var existing = await _context.Brands.FindAsync(brand.Id);
            if (existing == null) return false;

            existing.Name = brand.Name;
            existing.Slug = brand.Slug;
            existing.Country = brand.Country;
            existing.LogoPath = brand.LogoPath;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
