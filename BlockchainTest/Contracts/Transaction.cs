using System;

namespace BlockchainTest.Contracts
{
    public class Transaction
    {
        public DateTime Date { get; set; }

        public string Address { get; set; }

        public decimal Amount { get; set; }

        public int Confirmations { get; set; }
    }
}