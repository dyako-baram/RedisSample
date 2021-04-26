using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace RedisProject.Data
{
    public interface ISqlCommands
    {
        Task<IEnumerable<Sample1000Model>> AllData();
    }

    public class SqlCommands : ISqlCommands
    {
        private readonly IConfiguration _config;

        public SqlCommands(IConfiguration config)
        {
            _config = config;
        }
        public async Task<IEnumerable<Sample1000Model>> AllData()
        {
            using (var conn = new SqliteConnection(_config.GetConnectionString("Default")))
            {
                return await conn.QueryAsync<Sample1000Model>("Select id,name,Dob from SampleData;");
            }
        }
    }
}