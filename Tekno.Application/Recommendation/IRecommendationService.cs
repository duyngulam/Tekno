using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Application.Catalog.DTOs.Products;

namespace Tekno.Application.Recommendation
{
    public interface IRecommendationService
    {
        Task<(string category, string brand)> PredictAsync(int productId);
        Task<List<int>> RecommendCfAsync(int userId, int k = 10);
        Task<List<int>> RecommendContentAsync(int userId, int k = 10);
        Task<List<ProductSummaryDto>> RecommendCfWithProductsAsync(int userId, int k = 10);
        Task<List<ProductSummaryDto>> RecommendContentWithProductsAsync(int userId, int k = 10);
    }
}