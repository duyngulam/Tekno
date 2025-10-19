using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;

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
            return _mapper.Map<List<DTOs.BrandDto>>(brands);
        }
        public async Task<BrandDto> GetBrandBySlugAsync(string slug) {
            var brand = await _brandRepository.GetBrandBySlugAsync(slug);
            return _mapper.Map<BrandDto>(brand);
        }
    }
}
