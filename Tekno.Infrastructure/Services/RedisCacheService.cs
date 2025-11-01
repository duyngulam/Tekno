using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tekno.Application.Common.Cache;

namespace Tekno.Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _cache.GetStringAsync(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<T> CacheOrGetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null)
                return cached;

            var data = await factory();
            await SetAsync(key, data, expiry);
            return data;
        }
    }

}
