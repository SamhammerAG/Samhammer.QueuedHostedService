using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samhammer.QueuedHostedService
{
    public class QueuedHostedService : BackgroundService
    {
        private ILogger<QueuedHostedService> Logger { get; }

        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            TaskQueue = taskQueue;
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    Logger.LogDebug("Start executing queued background task.");
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error occurred executing queued background task.");
                }
            }
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            Logger.LogDebug("Queued Hosted Service is starting.");

            await base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogDebug("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
