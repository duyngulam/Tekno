using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Products;
using Tekno.Application.Catalog.Interface;
using AutoMapper;

namespace Tekno.Application.Recommendation
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationClient _client;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public RecommendationService(IRecommendationClient client, IProductRepository productRepository, IMapper mapper)
        {
            _client = client;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<(string category, string brand)> PredictAsync(int productId)
        {
            return await _client.PredictAsync(productId);
        }

        public async Task<List<int>> RecommendCfAsync(int userId, int k = 10)
        {
            return await _client.RecommendCfAsync(userId, k);
        }

        public async Task<List<int>> RecommendContentAsync(int userId, int k = 10)
        {
            return await _client.RecommendContentAsync(userId, k);
        }

        public async Task<List<ProductSummaryDto>> RecommendCfWithProductsAsync(int userId, int k = 10)
        {
            var ids = await _client.RecommendCfAsync(userId, k);
            if (ids == null || ids.Count == 0) return new List<ProductSummaryDto>();

            var products = await _productRepository.GetProductsByIdsAsync(ids);
            if (products == null || products.Count == 0) return new List<ProductSummaryDto>();

            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            // Enrich with rating stats
            var stats = await _productRepository.GetProductsRatingStatsAsync(dtos.Select(d => d.Id).ToList());
            foreach (var dto in dtos)
            {
                if (stats.TryGetValue(dto.Id, out var s))
                {
                    dto.AverageRating = s.AverageRating;
                    dto.TotalReviews = s.TotalReviews;
                }
                else
                {
                    dto.AverageRating = 0;
                    dto.TotalReviews = 0;
                }
            }

            // Preserve original recommendation order
            var dtoById = dtos.ToDictionary(d => d.Id);
            var ordered = ids.Where(id => dtoById.ContainsKey(id)).Select(id => dtoById[id]).ToList();
            return ordered;
        }

        public async Task<List<ProductSummaryDto>> RecommendContentWithProductsAsync(int userId, int k = 10)
        {
            var ids = await _client.RecommendContentAsync(userId, k);
            if (ids == null || ids.Count == 0) return new List<ProductSummaryDto>();

            var products = await _productRepository.GetProductsByIdsAsync(ids);
            if (products == null || products.Count == 0) return new List<ProductSummaryDto>();

            var dtos = _mapper.Map<List<ProductSummaryDto>>(products);

            // Enrich with rating stats
            var stats = await _productRepository.GetProductsRatingStatsAsync(dtos.Select(d => d.Id).ToList());
            foreach (var dto in dtos)
            {
                if (stats.TryGetValue(dto.Id, out var s))
                {
                    dto.AverageRating = s.AverageRating;
                    dto.TotalReviews = s.TotalReviews;
                }
                else
                {
                    dto.AverageRating = 0;
                    dto.TotalReviews = 0;
                }
            }

            // Preserve original recommendation order
            var dtoById = dtos.ToDictionary(d => d.Id);
            var ordered = ids.Where(id => dtoById.ContainsKey(id)).Select(id => dtoById[id]).ToList();
            return ordered;
        }
    }
}
