using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisProject.Data;
using RedisProject.Extension;

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
            var cacheKey = "DataList"; // we set key here; it will automaticly prefix it with RedisProject_ because we configured it in startup.cs
            var dataList = await _distributedCache.GetRecordAsync<List<Sample1000Model>>(cacheKey); //get data from redis;GetRecordAsync is Extension Method
            
            if (dataList == null) 
            {
                dataList =await _commands.AllData(); //get data from the database
                await _distributedCache.SetRecordAsync(cacheKey,dataList,expire:TimeSpan.FromMinutes(10),expireSlide:TimeSpan.FromMinutes(2));//SetRecordAsync is Extension Method
            }
            
            return Ok(dataList);
        }
    }
}
