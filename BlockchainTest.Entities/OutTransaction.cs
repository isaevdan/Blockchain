
using System;

namespace BlockchainTest.Entities
{
    public class OutTransaction
    {
        public string TxId { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        public DateTime TimeReceived { get; set; }

        public int Confirmations { get; set; }
        public string FromWallet { get; set; }
    }
}