using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Samhammer.QueuedHostedService
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> WorkItems { get; }

        public int ItemCount => WorkItems.Count;

        private SemaphoreSlim Signal { get; }

        public BackgroundTaskQueue()
        {
            WorkItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
            Signal = new SemaphoreSlim(0);
        }

        public void Enqueue(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            WorkItems.Enqueue(workItem);
            Signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await Signal.WaitAsync(cancellationToken);
            WorkItems.TryDequeue(out var workItem);

            return workItem;
        }
    }

    public interface IBackgroundTaskQueue
    {
        int ItemCount { get; }

        void Enqueue(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
