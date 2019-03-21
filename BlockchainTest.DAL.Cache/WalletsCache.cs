using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.DAL.Sql;
using BlockchainTest.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BlockchainTest.DAL.Cache
{
    public class WalletsCache: IWalletRepository
    {
        private readonly WalletRepository _repository;
        private readonly IMemoryCache _cache;

        public WalletsCache(WalletRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<Wallet> GetWalletForTransaction(decimal amount)
        {
            var wallets = await GetWallets();
            return wallets.FirstOrDefault(e => e.Balance > amount);
        }

        public async Task<Wallet[]> GetWallets()
        {
            if (_cache.TryGetValue(CacheKey(), out Wallet[] data))
                return data;

            data = await _repository.GetWallets();
            _cache.Set(CacheKey(), data);
            
            return data;
        }

        public async Task StoreWallets(Wallet[] wallets)
        {
            await _repository.StoreWallets(wallets);
            _cache.Set(CacheKey(), wallets);
        }
        
        private string CacheKey()
        {
            return GetType().ToString();
        }
    }
}