using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.DAL.Sql.Utils;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL.Sql
{
    public class WalletRepository : IWalletRepository
    {
        private readonly SqlConfiguration _configuration;

        public WalletRepository(SqlConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Wallet[]> GetWalletsForTransaction(decimal amount)
        {
            return (await SqlExecutor.ExecuteStoreProcedureReader(_configuration.ConnectionString,
                "Wallets_ForTransaction",
                dr => new Wallet()
                {
                    Balance = (decimal) dr["Balance"],
                    Name = (string) dr["Name"]
                },
                new[]
                {
                    new SqlParameter("@Amount", amount),
                }));
        }

        public async Task<Wallet[]> GetWallets()
        {
            return await SqlExecutor.ExecuteStoreProcedureReader(_configuration.ConnectionString,
                "Wallets_GetAll",
                dr => new Wallet()
                {
                    Balance = (decimal) dr["Balance"],
                    Name = (string) dr["Name"]
                });
        }

        async Task StoreWallet(Wallet wallet)
        {
            await SqlExecutor.ExecuteStoreProcedureScalar(_configuration.ConnectionString, "Wallet_Store",
                new[]
                {
                    new SqlParameter("@Name", wallet.Name),
                    new SqlParameter("@Balance", wallet.Balance)
                });
        }
        
        public async Task StoreWallets(Wallet[] wallets)
        {
            await Task.WhenAll(wallets.Select(StoreWallet));
        }
    }
}