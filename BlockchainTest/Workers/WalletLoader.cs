using System.Threading.Tasks;
using BlockchainTest.Services;
using Microsoft.Extensions.Logging;

namespace BlockchainTest.Workers
{
    class WalletLoader: BaseWorker
    {
        private readonly WalletService _walletService;

        public WalletLoader(Appsettings appsettings, WalletService walletService, ILogger<BaseWorker> logger) 
            : base(appsettings.LoadWalletsInterval, logger)
        {
            _walletService = walletService;
        }

        protected override async Task Worker()
        {
            await _walletService.UploadWallets();
        }
    }
}