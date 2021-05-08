using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryDemo.Services.Cache
{
    public interface ICacheService
    {
        Task<T> GetCacheValue<T>(string key);

        Task SetCacheValue<T>(string key, T value, TimeSpan? expiresIn = null);

        Task SetCacheValues<T>(IDictionary<string, T> values, TimeSpan? expiresIn = null);

        Task DeleteCacheValue(string key);
    }
}
