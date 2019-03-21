using System;
using System.Threading.Tasks;
using BlockchainTest.Entities;

namespace BlockchainTest.DAL
{
    public interface IOutputTransactionsRepository
    {
        Task StoreTransactions(OutTransaction[] transactions);
    }
}