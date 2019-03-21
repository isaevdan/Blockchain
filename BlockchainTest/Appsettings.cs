using System;
using BlockchainTest.Bitcoind;
using BlockchainTest.DAL.Sql;
using BlockchainTest.DAL.Sql.Utils;
using BlockchainTest.Services;
using BlockchainTest.Services.Configuration;

namespace BlockchainTest
{
    public class Appsettings
    {
        public BitcoindConfiguration Bitcoind { get; set; }
        public ServiceConfiguration Service { get; set; }
        public TimeSpan LoadTransactionsInterval { get; set; }
        public TimeSpan LoadWalletsInterval { get; set; }
        public SqlConfiguration Sql { get; set; }
    }
}