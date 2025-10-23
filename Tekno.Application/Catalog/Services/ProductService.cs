using AutoMapper;
using System.Text.Json;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.Interface;
using Tekno.Application.Common;
using Tekno.Application.Common.Paging;
using Tekno.Domain.Catalog;

namespace Tekno.Application.Catalog.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductSummaryDto>> GetPagedProductAsync(
            string? categorySlug,
            string? brandSlug,
            string? search,
            string? sort,
            string? minPrice,
            string? maxPrice,
            PagingParams paging)
        {
            var pagedResult = await _productRepository.GetPagedProductAsync(
                categorySlug,
                brandSlug,
                search,
                sort,
                minPrice,
                maxPrice,
                paging);

            var mapped = _mapper.Map<List<ProductSummaryDto>>(pagedResult.Data);

            return new PagedResult<ProductSummaryDto>(
                mapped,
                pagedResult.TotalRecords,
                paging.Page,
                paging.PageSize);
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(string slug)
        {
            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product == null) return null;
            return _mapper.Map<ProductDetailDto>(product);
        }
    }
}
