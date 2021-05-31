using Polly;
using Polly.Retry;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace InventoryDemo.Services.OrderExportCancellationHashs
{
    public class OrderExportCancellationHash : IOrderExportCancellationHash
    {
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _hash = new();

        private readonly RetryPolicy<bool> _policy = Policy.HandleResult(false).WaitAndRetry(5, attempt => TimeSpan.FromSeconds(1));

        public CancellationToken GetOrCreateCancellationToken(int orderExportId)
        {
            if (_hash.TryGetValue(orderExportId, out CancellationTokenSource tokenSource)) return tokenSource.Token;

            var cancellationTokenSource = new CancellationTokenSource();
            var added = _policy.Execute(() => _hash.TryAdd(orderExportId, cancellationTokenSource));

            if (!added)
                throw new Exception("Não foi possível adquirir o Token");

            return cancellationTokenSource.Token;
        }

        public void Cancel(int orderExportId)
        {
            if (!_hash.TryGetValue(orderExportId, out CancellationTokenSource tokenSource))
                throw new Exception("Não foi possível adquirir o Token");

            tokenSource.Cancel();
        }
    }
}
