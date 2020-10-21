using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samhammer.QueuedHostedService.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddBackgroundQueue();
                })
                .Build();

            await host.StartAsync();

            var backgroundQueue = host.Services.GetRequiredService<IBackgroundTaskQueue>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            
            backgroundQueue.Enqueue(token =>
            {
                logger.LogInformation("Task 1 executed.");
                return Task.CompletedTask;
            });

            backgroundQueue.Enqueue(token =>
            {
                logger.LogInformation("Task 2 executed.");
                return Task.CompletedTask;
            });

            backgroundQueue.Enqueue(token =>
            {
                logger.LogInformation("Task 3 executed.");
                return Task.CompletedTask;
            });

            await host.WaitForShutdownAsync();
        }
    }
}
