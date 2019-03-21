using System;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.Bitcoind;
using BlockchainTest.DAL;
using BlockchainTest.Entities;
using Microsoft.Extensions.Logging;

namespace BlockchainTest.Services
{
    public class WalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly BitcoindService _bitcoindService;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IWalletRepository walletRepository, BitcoindService bitcoindService,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _bitcoindService = bitcoindService;
            _logger = logger;
        }

        public async Task<Wallet> GetWalletForTransaction(decimal transactionAmount)
        {
            var wallets = await _walletRepository.GetWallets();
            return wallets.FirstOrDefault(e => e.Balance > transactionAmount);
        }

        public async Task<Wallet[]> GetAllWallets()
        {
            return await _walletRepository.GetWallets();
        }

        public async Task UploadWallets()
        {
            var walletNames = await _bitcoindService.ListWallets();
            if (!string.IsNullOrEmpty(walletNames.Error))
                throw new Exception(walletNames.Error);

            var walletResults = walletNames.Result.Select(e => (e, _bitcoindService.GetBalance(e))).ToArray();
            var wallets = await Task.WhenAll(walletResults.Select(async e =>
            {
                var balanceResult = await e.Item2;
                if (string.IsNullOrEmpty(balanceResult.Error))
                    return new Wallet()
                    {
                        Name = e.Item1,
                        Balance = balanceResult.Result
                    };
                else
                {
                    _logger.LogError(balanceResult.Error);
                    return null;
                }
            }).Where(e => e != null).ToArray());

            await _walletRepository.StoreWallets(wallets);
        }
    }
}