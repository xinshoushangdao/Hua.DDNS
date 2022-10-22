using System.Data;
using Dapper;
using Npgsql;

namespace Hua.DDNS.Common
{
    public class SqlHelper
    {

        private readonly string _connectionString;
        private readonly ILogger<SqlHelper> _logger;

        public SqlHelper(IConfiguration configuration, ILogger<SqlHelper> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("pgsql");
        }

        /// <summary>
        /// 查询所有结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSql"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> GetList<T>(string strSql)
        {
            var list = new List<T>();

            try
            {
                using IDbConnection connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                list = connection.Query<T>(strSql).ToList();
                connection.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
            return list;
        }

    }
}
