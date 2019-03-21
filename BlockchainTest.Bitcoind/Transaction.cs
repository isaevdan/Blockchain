namespace BlockchainTest.Bitcoind
{
    public class Transaction
    {
        public string TxId { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        public string Category { get; set; }

        public string Address { get; set; }

        public long TimeReceived { get; set; }

        public int Confirmations { get; set; }
    }

    public static class TransactionCategory
    {
        public const string Receive = "receive";
        public const string Send = "send";
    }

    public class BitcoinResponse<T>
    {
        public T Result { get; set; }
        public string Error { get; set; }
        public string Id { get; set; }
    }
}