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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {

            return await _context.Categories.AsNoTracking().ToListAsync();
        }

    }
}
