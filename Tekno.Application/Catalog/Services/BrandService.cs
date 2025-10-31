using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common.Cache;
using Tekno.Application.Common.Exceptions;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class BrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        public BrandService(IBrandRepository brandRepository, IMapper mapper, ICacheService cache)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            return await _cache.CacheOrGetAsync(
            CachePolicies.BrandKey,
            async () => _mapper.Map<List<BrandDto>>(await _brandRepository.GetAllBrandsAsync()),
            CachePolicies.BrandTtl
        );
        }
        public async Task<BrandDto> GetBrandBySlugAsync(string slug) {
            var brand = await _brandRepository.GetBrandBySlugAsync(slug);
            return _mapper.Map<BrandDto>(brand);
        }
        public async Task<BrandDto> GetBrandByIdAsync(int id)
        {
            var brand = await _brandRepository.GetBrandByIdAsync(id);
            return _mapper.Map<BrandDto>(brand);
        }
        public async Task<Brand> CreateAsync(BrandDto brandDto)
        {
            if(await _brandRepository.GetBrandBySlugAsync(brandDto.Slug) != null)
            {
                throw new ConflictException($"Brand '{brandDto.Slug}' already exists.", "BRAND_DUPLICATE");
            }
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
