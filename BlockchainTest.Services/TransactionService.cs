using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainTest.Bitcoind;
using BlockchainTest.DAL;
using BlockchainTest.Entities;
using BlockchainTest.Services.Configuration;

namespace BlockchainTest.Services
{
    public class TransactionService
    {
        private readonly BitcoindService _bitcoindService;
        private readonly WalletService _walletService;
        private readonly IInputTransactionsRepository _inputTransactionsRepository;
        private readonly IOutputTransactionsRepository _outputTransactionsRepository;
        private readonly ServiceConfiguration _serviceConfiguration;

        public TransactionService(BitcoindService bitcoindService, 
                                  WalletService walletService, 
                                  IInputTransactionsRepository inputTransactionsRepository,
                                  IOutputTransactionsRepository outputTransactionsRepository,
                                  ServiceConfiguration serviceConfiguration)
        {
            _bitcoindService = bitcoindService;
            _walletService = walletService;
            _inputTransactionsRepository = inputTransactionsRepository;
            _outputTransactionsRepository = outputTransactionsRepository;
            _serviceConfiguration = serviceConfiguration;
        }

        public async Task<bool> SendBtc(string address, decimal amount)
        {
            var wallets = await _walletService.GetWalletsForTransaction(amount);
            foreach (var wallet in wallets)
            {
                var result = await _bitcoindService.SendToAddress(wallet.Name, address, amount);

                if (string.IsNullOrEmpty(result.Error) && !string.IsNullOrEmpty(result.Result))
                    return true;
            }

            return false;
        }

        public async Task<InTransaction[]> GetLast()
        {
            return await _inputTransactionsRepository.GetLast();
        }

        public async Task UploadTransactions()
        {
            var wallets = await _walletService.GetAllWallets();
            var walletsTransactions = await Task.WhenAll(wallets.Select(GetNewTransactionsForWallet).ToArray());
            
            List<InTransaction> inTransactions = new List<InTransaction>();
            List<OutTransaction> outTransactions = new List<OutTransaction>();
            foreach (var walletsTransaction in walletsTransactions)
            {
                inTransactions.AddRange(walletsTransaction.Item1);
                outTransactions.AddRange(walletsTransaction.Item2);
            }

            await Task.WhenAll(_inputTransactionsRepository.StoreTransactions(inTransactions.ToArray()),
                _outputTransactionsRepository.StoreTransactions(outTransactions.ToArray()));
        }
        
        private async Task<(InTransaction[], OutTransaction[])> GetNewTransactionsForWallet(Wallet wallet)
        {
            List<InTransaction> inTransactions = new List<InTransaction>();
            List<OutTransaction> outTransactions = new List<OutTransaction>();
            for (int i = 0; ; i+= _serviceConfiguration.PullTransactionStep)
            {
                var transactionResult =
                    await _bitcoindService.ListTransactions(wallet.Name, _serviceConfiguration.PullTransactionStep, i);
                if(!string.IsNullOrEmpty(transactionResult.Error))
                    break;
                
                var transactionBatch = transactionResult.Result;
                if(transactionBatch.Length == 0)
                    break;

                foreach (var transaction in transactionBatch)
                {
                    switch (transaction.Category)
                    {
                        case TransactionCategory.Receive:
                            inTransactions.Add(ToInTransaction(transaction));
                            break;
                        case TransactionCategory.Send:
                            var outTransaction = ToOutTransaction(transaction, wallet.Name);
                            outTransactions.Add(outTransaction);
                            break;
                    }
                }
                
                if(transactionBatch.Length < _serviceConfiguration.PullTransactionStep)
                    break;
            }

            return (inTransactions.ToArray(), outTransactions.ToArray());
        }

        private InTransaction ToInTransaction(Transaction transaction)
        {
            return new InTransaction()
            {
                Amount = transaction.Amount,
                Address = transaction.Address,
                Confirmations = transaction.Confirmations,
                TimeReceived = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(transaction.TimeReceived),
                TxId = transaction.TxId
            };
        }
        
        private OutTransaction ToOutTransaction(Transaction transaction, string walletName)
        {
            return new OutTransaction()
            {
                Amount = transaction.Amount,
                Address = transaction.Address,
                Confirmations = transaction.Confirmations,
                TimeReceived = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(transaction.TimeReceived),
                TxId = transaction.TxId,
                FromWallet = walletName
            };
        }
    }
}