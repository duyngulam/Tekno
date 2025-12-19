using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Location.Interface;
using Tekno.Domain.Location;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Location
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Province>> GetAllProvincesAsync()
        {
            return await _context.Provinces
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<District>> GetDistrictsByProvinceCodeAsync(int provinceCode)
        {
            return await _context.Districts
                .Where(d => d.ProvinceCode == provinceCode)
                .OrderBy(d => d.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Ward>> GetWardsByDistrictCodeAsync(int districtCode)
        {
            return await _context.Wards
                .Where(w => w.DistrictCode == districtCode)
                .OrderBy(w => w.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Province?> GetProvinceByCodeAsync(int code)
        {
            return await _context.Provinces
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<District?> GetDistrictByCodeAsync(int code)
        {
            return await _context.Districts
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<Ward?> GetWardByCodeAsync(int code)
        {
            return await _context.Wards
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Code == code);
        }

        public async Task<List<Province>> SearchProvincesAsync(string keyword)
        {
            return await _context.Provinces
                .Where(p => EF.Functions.ILike(p.Name, $"%{keyword}%") || 
                           EF.Functions.ILike(p.Codename, $"%{keyword}%"))
                .OrderBy(p => p.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddProvinceAsync(Province province)
        {
            await _context.Provinces.AddAsync(province);
        }

        public async Task AddDistrictAsync(District district)
        {
            await _context.Districts.AddAsync(district);
        }

        public async Task AddWardAsync(Ward ward)
        {
            await _context.Wards.AddAsync(ward);
        }

        public async Task<bool> ProvinceExistsByCodeAsync(int code)
        {
            return await _context.Provinces.AnyAsync(p => p.Code == code);
        }

        public async Task<bool> DistrictExistsByCodeAsync(int code)
        {
            return await _context.Districts.AnyAsync(d => d.Code == code);
        }

        public async Task<bool> WardExistsByCodeAsync(int code)
        {
            return await _context.Wards.AnyAsync(w => w.Code == code);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
