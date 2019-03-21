using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BlockchainTest.Workers
{
    abstract class BaseWorker
    {
        private readonly TimeSpan _period;
        private readonly ILogger<BaseWorker> _logger;
        private readonly Timer _timer;

        protected BaseWorker(TimeSpan period, ILogger<BaseWorker> logger)
        {
            _period = period;
            _logger = logger;
            _timer = new Timer(async o => await DoWork(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        protected abstract Task Worker();

        private async Task DoWork()
        {
            _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            try
            {
                await Worker();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in " + GetType());
            }

            _timer.Change(_period, _period);
        }

        public void Start()
        {
            _timer.Change(TimeSpan.Zero, _period);
        }
    }
}