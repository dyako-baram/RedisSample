using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisProject.Data;

namespace RedisProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ISqlCommands _commands;

        public SampleController(ISqlCommands commands)
        {
            _commands = commands;
        }

        [HttpGet]
        public IActionResult GetAllData()
        {
            return Ok (_commands.AllData());
        }
    }
}
