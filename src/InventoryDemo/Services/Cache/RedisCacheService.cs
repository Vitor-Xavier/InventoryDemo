using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer) => _connectionMultiplexer = connectionMultiplexer;

        public async Task<T> GetCacheValue<T>(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            string value = await database.StringGetAsync(key);
            return value is null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetCacheValue<T>(string key, T value, TimeSpan? expiresIn = null)
        {
            var database = _connectionMultiplexer.GetDatabase();
            string cacheValue = JsonSerializer.Serialize(value);
            await database.StringSetAsync(key, cacheValue, expiresIn);
        }

        public async Task SetCacheValues<T>(IDictionary<string, T> values, TimeSpan? expiresIn = null)
        {
            var database = _connectionMultiplexer.GetDatabase();
            foreach (var value in values)
                await database.StringSetAsync(value.Key, JsonSerializer.Serialize(value.Value), expiresIn);
        }

        public async Task DeleteCacheValue(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            await database.KeyDeleteAsync(key);
        }
    }
}
