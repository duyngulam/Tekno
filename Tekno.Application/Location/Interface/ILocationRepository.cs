using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Domain.Location;

namespace Tekno.Application.Location.Interface
{
    public interface ILocationRepository
    {
        Task<List<Province>> GetAllProvincesAsync();
        Task<List<District>> GetDistrictsByProvinceCodeAsync(int provinceCode);
        Task<List<Ward>> GetWardsByDistrictCodeAsync(int districtCode);
        Task<Province?> GetProvinceByCodeAsync(int code);
        Task<District?> GetDistrictByCodeAsync(int code);
        Task<Ward?> GetWardByCodeAsync(int code);
        Task<List<Province>> SearchProvincesAsync(string keyword);
        Task AddProvinceAsync(Province province);
        Task AddDistrictAsync(District district);
        Task AddWardAsync(Ward ward);
        Task<bool> ProvinceExistsByCodeAsync(int code);
        Task<bool> DistrictExistsByCodeAsync(int code);
        Task<bool> WardExistsByCodeAsync(int code);
        Task SaveChangesAsync();
    }
}
