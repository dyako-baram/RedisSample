using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RedisProject.Data;

namespace RedisProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ISqlCommands _commands;
        private readonly IDistributedCache _distributedCache;

        public SampleController(ISqlCommands commands, IDistributedCache distributedCache)
        {
            _commands = commands;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var cacheKey = "DataList"; // we set key here
            string serializedDataList; 
            var DataList = new List<Sample1000Model>(); //this will be populated either from the cache or database

            var redisDataList = await _distributedCache.GetAsync(cacheKey); //get data from redis

            if (redisDataList != null) // if there is data from redis
            {
                serializedDataList = Encoding.UTF8.GetString(redisDataList);
                DataList=JsonSerializer.Deserialize<List<Sample1000Model>>(serializedDataList);
            }
            else
            {
                DataList =await _commands.AllData(); //get data from the database
                serializedDataList = JsonSerializer.Serialize(DataList); 
                redisDataList = Encoding.UTF8.GetBytes(serializedDataList);
                var options = new DistributedCacheEntryOptions(){ //options for how it should cache data
                    AbsoluteExpirationRelativeToNow=TimeSpan.FromMinutes(10), // time for expireation
                    SlidingExpiration=TimeSpan.FromMinutes(2) // if we dont use this cached data after 2 minute it will automaticly expire but if you use it before 2 min it will reset the 2 min timer
                };
                await _distributedCache.SetAsync(cacheKey, redisDataList, options);
            }
            return Ok(DataList);
        }
    }
}
