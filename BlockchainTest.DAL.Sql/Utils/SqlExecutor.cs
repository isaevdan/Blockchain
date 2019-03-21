using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BlockchainTest.DAL.Sql.Utils
{
    public static class SqlExecutor
    {
        public static async Task<T[]> ExecuteStoreProcedureReader<T>(string connectionString, string procedure, Func<IDataReader, T> mapper, SqlParameter[] parameters = null)
        {
            List<T> result = new List<T>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = procedure;
                command.CommandType = CommandType.StoredProcedure;
                
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    result.Add(mapper(reader));
                }
            }

            return result.ToArray();
        }

        public static async Task ExecuteStoreProcedureScalar(string connectionString, string procedure,
            SqlParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = procedure;
                command.CommandType = CommandType.StoredProcedure;
                if (parameters != null)
                    command.Parameters.AddRange(parameters);
                await command.ExecuteScalarAsync();
            }
        }
    }
}