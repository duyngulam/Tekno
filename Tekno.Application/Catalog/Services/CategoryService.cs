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
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
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
    }
}
