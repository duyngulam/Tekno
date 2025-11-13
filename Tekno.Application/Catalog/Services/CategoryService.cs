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
using Tekno.Application.Common.Media.Services;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly MediaService _mediaService;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cache = cacheService;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _cache.CacheOrGetAsync(
                CachePolicies.CategoryKey,
                async () => _mapper.Map<List<CategoryDto>>(await _categoryRepository.GetAllCategoriesAsync()),
                CachePolicies.CategoryTtl
            );
        }

        public async Task<List<CategoryTreeDto>> GetCategoryTreeAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            var dtoList = _mapper.Map<List<CategoryDto>>(categories);

            var lookup = dtoList.ToLookup(c => c.ParentId);

            List<CategoryTreeDto> BuildTree(int? parentId)
            {
                return lookup[parentId]
                    .Select(c => new CategoryTreeDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Slug = c.Slug,
                        IconPath = c.IconPath,
                        SubCategories = BuildTree(c.Id)
                    }).ToList();
            }

            return BuildTree(null); // categories cha (ParentId = null)
        }

        public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            var category = await _categoryRepository.GetCategoryBySlugAsync(slug);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<Category> CreateAsync(CategoryDto categoryDto)
        {
            if (await _categoryRepository.GetCategoryBySlugAsync(categoryDto.Slug) != null)
            {
                throw new ConflictException($"Category '{categoryDto.Slug}' already exists.", "CATEGORY_DUPLICATE");
            }
            var category = _mapper.Map<Category>(categoryDto);
            return await _categoryRepository.CreateAsync(category);
        }

        public async Task<bool> UpdateAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task<bool> DeleteAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            return await _categoryRepository.DeleteAsync(category.Id);
        }
    }
}
