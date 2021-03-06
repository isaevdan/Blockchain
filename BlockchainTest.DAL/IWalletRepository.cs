using System.Threading.Tasks;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL
{
    public interface IWalletRepository
    {
        Task<Wallet[]> GetWalletsForTransaction(decimal amount);
        Task<Wallet[]> GetWallets();
        Task StoreWallets(Wallet[] wallets);
    }
}