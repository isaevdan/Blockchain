using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.DAL.Sql;
using BlockchainTest.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BlockchainTest.DAL.Cache
{
    public class InputTransactionsCache : IInputTransactionsRepository
    {
        private readonly InputTransactionsRepository _repository;
        private readonly IMemoryCache _cache;

        public InputTransactionsCache(InputTransactionsRepository repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task StoreTransactions(InTransaction[] transactions)
        {
            await _repository.StoreTransactions(transactions);
            _cache.Remove(CacheKey());
        }

        public async Task<InTransaction[]> GetLast()
        {
            if (_cache.TryGetValue(CacheKey(), out InTransaction[] transactions))
                return transactions;

            transactions = await _repository.GetLast();
            _cache.Set(CacheKey(), transactions.Where(e => e.Confirmations < 3).ToArray());
            
            return transactions;
        }

        private string CacheKey()
        {
            return GetType().ToString();
        }
    }
}