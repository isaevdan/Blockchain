using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockchainTest.Bitcoind
{
    public class BitcoindService
    {
        private readonly BitcoindConfiguration _configuration;

        public BitcoindService(BitcoindConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetWalletEndpoint(string endpoint, string walletName)
        {
            return walletName != null
                ? $"{endpoint}/wallet/{Uri.EscapeDataString(walletName)}"
                : endpoint;
        }

        private async Task<T> ExecuteRequest<T>(string walletName, string method, object[] parameters)
        {
            return await BitcoindClient.SendRequest<T>(_configuration.RpcUsername, _configuration.RpcPassword, GetWalletEndpoint(_configuration.Address, walletName), method, parameters);
        }
        
        public async Task<BitcoinResponse<Transaction[]>> ListTransactions(string walletName, int top, int skip)
        {
            return await ExecuteRequest<BitcoinResponse<Transaction[]>>(walletName, "listtransactions", new object[] {"*", top, skip});
        }
        
        public async Task<BitcoinResponse<string[]>> ListWallets()
        {
            return await ExecuteRequest<BitcoinResponse<string[]>>(string.Empty, "listwallets", null);
        }
        
        public async Task<BitcoinResponse<decimal>> GetBalance(string wallet)
        {
            return await ExecuteRequest<BitcoinResponse<decimal>>(wallet, "getbalance", null);
        }

        public async Task<BitcoinResponse<string>> SendToAddress(string walletName, string address, decimal amount)
        {
            return await ExecuteRequest<BitcoinResponse<string>>(walletName, "sendtoaddress", new object[] {address, amount});
        }
    }
}