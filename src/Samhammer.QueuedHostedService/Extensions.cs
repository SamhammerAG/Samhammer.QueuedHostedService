using Microsoft.Extensions.DependencyInjection;

namespace Samhammer.QueuedHostedService
{
    public static class Extensions
    {
        public static void AddBackgroundQueue(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        }
    }
}
