using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tekno.Application.Recommendation
{
    public interface IRecommendationClient
    {
        Task<(string category, string brand)> PredictAsync(int productId);
        Task<List<int>> RecommendCfAsync(int userId, int k = 10);
        Task<List<int>> RecommendContentAsync(int userId, int k = 10);
    }
}