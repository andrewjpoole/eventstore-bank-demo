using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.StatisticsGatherer
{
    public class StatisticsTaskQueue : IStatisticsTaskQueue
    {
        private ConcurrentQueue<(DateTime ResponseSent, long durationInTicks)> _durationRecordings = new ConcurrentQueue<(DateTime ResponseSent, long durationInTicks)>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueStatisticsWorkItem((DateTime ResponseSent, long durationInTicks) durationRecording)
        {
            _durationRecordings.Enqueue(durationRecording);
            _signal.Release();
        }

        public async Task<(DateTime ResponseSent, long durationInTicks)> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _durationRecordings.TryDequeue(out var durationRecording);

            return durationRecording;
        }
    }
}
