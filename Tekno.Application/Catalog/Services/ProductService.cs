using AutoMapper;
using System.Globalization;
using Tekno.Application.Catalog.DTOs;
using Tekno.Application.Catalog.DTOs.Products;
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
        private readonly IElasticProductService _elasticService;

        public ProductService(IProductRepository productRepository, IMapper mapper, IElasticProductService elasticService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _elasticService = elasticService;
        }
        public async Task<PagedResult<ProductSummaryDto>> GetPagedProductAsync(ProductSearchRequestDto request)
        {
            var paging = new PagingParams(request.Page, request.PageSize);

            // Use ES when keyword or spec filters present (keeps previous behavior)
            if (!string.IsNullOrEmpty(request.Keyword) || (request.Filters != null && request.Filters.Any()))
            {
                var elasticResult = await _elasticService.SearchProductsAsync(
                    request.Keyword,
                    request.Category,
                    request.Brand,
                    request.Filters,
                    request.MinPrice,
                    request.MaxPrice,
                    request.Sort,
                    paging.Page,
                    paging.PageSize);

                return elasticResult;
            }

            // Fallback to database
            var pagedResult = await _productRepository.GetPagedProductAsync(
                request.Category,
                request.Brand,
                null,
                request.Sort,
                request.MinPrice?.ToString(),
                request.MaxPrice?.ToString(),
                paging);

            var mapped = _mapper.Map<List<ProductSummaryDto>>(pagedResult.Data);
            return new PagedResult<ProductSummaryDto>(mapped, pagedResult.TotalRecords, paging.Page, paging.PageSize);
        }


        public async Task<ProductDetailDto?> GetProductDetailAsync(string slug)
        {
            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product == null) return null;
            return _mapper.Map<ProductDetailDto>(product);
        }
    }
}
