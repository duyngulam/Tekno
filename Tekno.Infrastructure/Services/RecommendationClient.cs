using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Tekno.Application.Recommendation;

namespace Tekno.Infrastructure.Services
{
    public class RecommendationClient : IRecommendationClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public RecommendationClient(HttpClient http, Microsoft.Extensions.Configuration.IConfiguration cfg)
        {
            _http = http;
            _baseUrl = cfg["TRAINING_API_URL"] ?? "http://localhost:8000";
        }

        public async Task<(string category, string brand)> PredictAsync(int productId)
        {
            var payload = new { product_id = productId };
            var resp = await _http.PostAsJsonAsync(new Uri(new Uri(_baseUrl), "/predict"), payload);
            resp.EnsureSuccessStatusCode();
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var category = doc.GetProperty("category").GetString() ?? string.Empty;
            var brand = doc.GetProperty("brand").GetString() ?? string.Empty;
            return (category, brand);
        }

        public async Task<List<int>> RecommendCfAsync(int userId, int k = 10)
        {
            var url = $"{_baseUrl.TrimEnd('/')}/recommend/cf/{userId}?k={k}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var list = new List<int>();
            if (doc.TryGetProperty("recommendations", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray()) list.Add(item.GetInt32());
            }
            return list;
        }

        public async Task<List<int>> RecommendContentAsync(int userId, int k = 10)
        {
            var url = $"{_baseUrl.TrimEnd('/')}/recommend/content/{userId}?k={k}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var list = new List<int>();
            if (doc.TryGetProperty("recommendations", out var arr) && arr.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in arr.EnumerateArray()) list.Add(item.GetInt32());
            }
            return list;
        }
    }
}