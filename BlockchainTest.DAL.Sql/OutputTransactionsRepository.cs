using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.DAL.Sql.Utils;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL.Sql
{
    public class OutputTransactionsRepository: IOutputTransactionsRepository
    {
        private readonly SqlConfiguration _configuration;

        public OutputTransactionsRepository(SqlConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task StoreTransactions(OutTransaction[] transactions)
        {
            await Task.WhenAll(transactions.Select(StoreTransaction));
        }

        private async Task StoreTransaction(OutTransaction transaction)
        {
            await SqlExecutor.ExecuteStoreProcedureScalar(_configuration.ConnectionString, "OutTransactions_Store",
                new[]
                {
                    new SqlParameter("@TxId", transaction.TxId),
                    new SqlParameter("@Amount", transaction.Amount),
                    new SqlParameter("@Address", transaction.Address),
                    new SqlParameter("@TimeReceived", transaction.TimeReceived),
                    new SqlParameter("@Confirmations", transaction.Confirmations),
                    new SqlParameter("@FromWallet", transaction.FromWallet)
                });
        }
    }
}