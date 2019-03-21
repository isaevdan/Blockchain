using System.Threading.Tasks;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL
{
    public interface IInputTransactionsRepository
    {
        Task StoreTransactions(InTransaction[] transactions);
        Task<InTransaction[]> GetLast();
    }
}