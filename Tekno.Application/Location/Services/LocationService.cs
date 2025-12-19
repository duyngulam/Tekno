using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tekno.Application.Common.Interfaces;
using Tekno.Application.Location.DTOs;
using Tekno.Application.Location.Interface;
using Tekno.Domain.Location;

namespace Tekno.Application.Location.Services
{
    public class LocationService
    {
        private readonly ILocationRepository _repository;
        private readonly IMapper _mapper;
        private readonly IAppLogger<LocationService> _logger;

        public LocationService(ILocationRepository repository, IMapper mapper, IAppLogger<LocationService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Import provinces, districts, and wards from JSON file
        /// </summary>
        public async Task<ImportResultDto> ImportFromJsonFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"JSON file not found: {filePath}");

            var json = await File.ReadAllTextAsync(filePath);
            var data = JsonSerializer.Deserialize<List<JsonElement>>(json);

            if (data == null || !data.Any())
                throw new InvalidOperationException("JSON file is empty or invalid");

            int provincesCount = 0, districtsCount = 0, wardsCount = 0;

            foreach (var provinceJson in data)
            {
                var provinceCode = provinceJson.GetProperty("code").GetInt32();
                var provinceName = provinceJson.GetProperty("name").GetString() ?? string.Empty;
                var provinceCodename = provinceJson.TryGetProperty("codename", out var cn) ? cn.GetString() ?? string.Empty : string.Empty;
                var provinceDivisionType = provinceJson.TryGetProperty("division_type", out var dt) ? dt.GetString() ?? string.Empty : string.Empty;
                var phoneCode = provinceJson.TryGetProperty("phone_code", out var pc) ? (int?)pc.GetInt32() : null;

                // Add province if not exists
                if (!await _repository.ProvinceExistsByCodeAsync(provinceCode))
                {
                    var province = new Province(provinceCode, provinceName, provinceCodename, provinceDivisionType, phoneCode);
                    await _repository.AddProvinceAsync(province);
                    provincesCount++;
                }

                // Process districts
                if (provinceJson.TryGetProperty("districts", out var districtsArray))
                {
                    foreach (var districtJson in districtsArray.EnumerateArray())
                    {
                        var districtCode = districtJson.GetProperty("code").GetInt32();
                        var districtName = districtJson.GetProperty("name").GetString() ?? string.Empty;
                        var districtCodename = districtJson.TryGetProperty("codename", out var dcn) ? dcn.GetString() ?? string.Empty : string.Empty;
                        var districtDivisionType = districtJson.TryGetProperty("division_type", out var ddt) ? ddt.GetString() ?? string.Empty : string.Empty;

                        // Add district if not exists
                        if (!await _repository.DistrictExistsByCodeAsync(districtCode))
                        {
                            var district = new District(districtCode, districtName, districtCodename, districtDivisionType, provinceCode);
                            await _repository.AddDistrictAsync(district);
                            districtsCount++;
                        }

                        // Process wards
                        if (districtJson.TryGetProperty("wards", out var wardsArray))
                        {
                            foreach (var wardJson in wardsArray.EnumerateArray())
                            {
                                var wardCode = wardJson.GetProperty("code").GetInt32();
                                var wardName = wardJson.GetProperty("name").GetString() ?? string.Empty;
                                var wardCodename = wardJson.TryGetProperty("codename", out var wcn) ? wcn.GetString() ?? string.Empty : string.Empty;
                                var wardDivisionType = wardJson.TryGetProperty("division_type", out var wdt) ? wdt.GetString() ?? string.Empty : string.Empty;

                                // Add ward if not exists
                                if (!await _repository.WardExistsByCodeAsync(wardCode))
                                {
                                    var ward = new Ward(wardCode, wardName, wardCodename, wardDivisionType, districtCode);
                                    await _repository.AddWardAsync(ward);
                                    wardsCount++;
                                }
                            }
                        }
                    }
                }
            }

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Imported {Provinces} provinces, {Districts} districts, {Wards} wards", 
                provincesCount, districtsCount, wardsCount);

            return new ImportResultDto
            {
                ProvincesImported = provincesCount,
                DistrictsImported = districtsCount,
                WardsImported = wardsCount,
                Message = $"Successfully imported {provincesCount} provinces, {districtsCount} districts, and {wardsCount} wards"
            };
        }

        /// <summary>
        /// Get all provinces
        /// </summary>
        public async Task<List<ProvinceDto>> GetAllProvincesAsync()
        {
            var provinces = await _repository.GetAllProvincesAsync();
            return _mapper.Map<List<ProvinceDto>>(provinces);
        }

        /// <summary>
        /// Get districts by province code
        /// </summary>
        public async Task<List<DistrictDto>> GetDistrictsByProvinceAsync(int provinceCode)
        {
            var districts = await _repository.GetDistrictsByProvinceCodeAsync(provinceCode);
            return _mapper.Map<List<DistrictDto>>(districts);
        }

        /// <summary>
        /// Get wards by district code
        /// </summary>
        public async Task<List<WardDto>> GetWardsByDistrictAsync(int districtCode)
        {
            var wards = await _repository.GetWardsByDistrictCodeAsync(districtCode);
            return _mapper.Map<List<WardDto>>(wards);
        }

        /// <summary>
        /// Search provinces by keyword
        /// </summary>
        public async Task<List<ProvinceDto>> SearchProvincesAsync(string keyword)
        {
            var provinces = await _repository.SearchProvincesAsync(keyword);
            return _mapper.Map<List<ProvinceDto>>(provinces);
        }
    }
}
