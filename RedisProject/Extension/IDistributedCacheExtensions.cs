using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisProject.Extension
{
    public static class IDistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,
            string cacheKey,
            T data,
            TimeSpan? expire=null
            ,TimeSpan? expireSlide=null)
        {
            string jsonString = JsonSerializer.Serialize(data); 
            var options = new DistributedCacheEntryOptions(){ //options for how it should cache data
                AbsoluteExpirationRelativeToNow=expire ?? TimeSpan.FromMinutes(10), // time for expireation
                SlidingExpiration=expireSlide // if we dont use this cached data after 2 minute it will automaticly expire but if you use it before 2 min it will reset the 2 min timer
            };
            await cache.SetStringAsync(cacheKey, jsonString, options);
        }
        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache,string cacheKey)
        {
            var jsonData=await cache.GetStringAsync(cacheKey);//get data from redis by key
            if(jsonData is null)
            {
                return default(T);//return default value of tye type that is being passed
            }
            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}