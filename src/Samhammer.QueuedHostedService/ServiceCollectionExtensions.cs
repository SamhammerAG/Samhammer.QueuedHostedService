using Microsoft.Extensions.DependencyInjection;

namespace Samhammer.QueuedHostedService
{
    public static class ServiceCollectionExtensions
    {
        public static void AddBackgroundQueue(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
