using System.Threading.Tasks;
using BlockchainTest.Services;
using Microsoft.Extensions.Logging;

namespace BlockchainTest.Workers
{
    class TransactionsLoader : BaseWorker
    {
        private readonly TransactionService _transactionService;

        public TransactionsLoader(TransactionService transactionService, Appsettings appsettings, ILogger<BaseWorker> logger)
            : base(appsettings.LoadTransactionsInterval, logger)
        {
            _transactionService = transactionService;
        }


        protected override async Task Worker()
        {
            await _transactionService.UploadTransactions();
        }
    }
}