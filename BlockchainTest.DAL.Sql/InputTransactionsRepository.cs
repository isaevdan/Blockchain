using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.DAL.Sql.Utils;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL.Sql
{
    public class InputTransactionsRepository : IInputTransactionsRepository
    {
        private readonly SqlConfiguration _configuration;

        public InputTransactionsRepository(SqlConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StoreTransactions(InTransaction[] transactions)
        {
            await Task.WhenAll(transactions.Select(StoreTransaction));
        }

        private async Task StoreTransaction(InTransaction transaction)
        {
            await SqlExecutor.ExecuteStoreProcedureScalar(_configuration.ConnectionString, "InTransactions_Store",
                new[]
                {
                    new SqlParameter("@TxId", transaction.TxId),
                    new SqlParameter("@Amount", transaction.Amount),
                    new SqlParameter("@Address", transaction.Address),
                    new SqlParameter("@TimeReceived", transaction.TimeReceived),
                    new SqlParameter("@Confirmations", transaction.Confirmations)
                });
        }

        public async Task<InTransaction[]> GetLast()
        {
            return await SqlExecutor.ExecuteStoreProcedureReader(_configuration.ConnectionString,
                "InTransactions_GetLast",
                dr => new InTransaction()
                {
                    TxId = (string) dr["TxId"],
                    Address = (string) dr["Address"],
                    TimeReceived = (DateTime) dr["TimeReceived"],
                    Confirmations = (int) dr["Confirmations"],
                    Amount = (decimal) dr["Amount"]
                });
        }
    }
}