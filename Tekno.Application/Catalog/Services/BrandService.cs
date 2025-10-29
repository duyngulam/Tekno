using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class BrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        public BrandService(IBrandRepository brandRepository, IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }
        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllBrandsAsync();
            return _mapper.Map<List<BrandDto>>(brands);
        }
        public async Task<BrandDto> GetBrandBySlugAsync(string slug) {
            var brand = await _brandRepository.GetBrandBySlugAsync(slug);
            return _mapper.Map<BrandDto>(brand);
        }
        public async Task<Brand> CreateAsync(BrandDto brandDto)
        {
            var brand = _mapper.Map<Brand>(brandDto);
            return await _brandRepository.CreateAsync(brand);
        }
        public async Task<bool> UpdateAsync(BrandDto brandDto)
        {
            var brand = _mapper.Map<Brand>(brandDto);
            return await _brandRepository.UpdateAsync(brand);
        }
        public async Task<bool> DeleteAsync(BrandDto brandDto)
        {
            var brand = _mapper.Map<Brand>(brandDto);
            return await _brandRepository.DeleteAsync(brand.Id);
        }
    }
}
