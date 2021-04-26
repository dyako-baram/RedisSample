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
            var cacheKey = "DataList";
            string serializedCustomerList;
            var DataList = new List<Sample1000Model>();
            var redisDataList = await _distributedCache.GetAsync(cacheKey);
            if (redisDataList != null)
            {
                serializedCustomerList = Encoding.UTF8.GetString(redisDataList);
                DataList=JsonSerializer.Deserialize<List<Sample1000Model>>(serializedCustomerList);
            }
            else
            {
                DataList =await _commands.AllData();
                serializedCustomerList = JsonSerializer.Serialize(DataList);
                redisDataList = Encoding.UTF8.GetBytes(serializedCustomerList);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddSeconds(15))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(7));
                await _distributedCache.SetAsync(cacheKey, redisDataList, options);
            }
            return Ok(DataList);
        }
    }
}
