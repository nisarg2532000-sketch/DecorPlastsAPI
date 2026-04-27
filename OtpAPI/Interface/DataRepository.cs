using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace DecorPlastsAPI.Interface
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<T> Query<T>(string storedProcedure, dynamic param = null)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.Query<T>(storedProcedure, param: (object?)param, commandType: CommandType.StoredProcedure);
        }

        public int ExecuteSP(string storedProcedure, dynamic param = null)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.Execute(storedProcedure, param: (object?)param, commandType: CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultipleSP<T1, T2>(string storedProcedure, dynamic param = null)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var multi = connection.QueryMultiple(storedProcedure, param: (object?)param, commandType: CommandType.StoredProcedure);
            var result1 = multi.Read<T1>();
            var result2 = multi.Read<T2>();
            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(result1, result2);
        }

        public T QueryFirstOrDefault<T>(string storedProcedure, dynamic param = null)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection.QueryFirstOrDefault<T>(storedProcedure, param: (object?)param, commandType: CommandType.StoredProcedure);
        }
    }
}

