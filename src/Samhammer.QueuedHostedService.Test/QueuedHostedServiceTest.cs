using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Samhammer.QueuedHostedService.Test
{
    public class QueuedHostedServiceTest
    {
        private IBackgroundTaskQueue BackgroundQueue { get; }

        public QueuedHostedServiceTest()
        {
            var services = new ServiceCollection();
            services.AddBackgroundQueue();

            var serviceProvider = services.BuildServiceProvider();
            BackgroundQueue = serviceProvider.GetService<IBackgroundTaskQueue>();
        }

        [Fact]
        public async Task QueuedTasksExecuteInOrder()
        {
            var generated = new List<int>();
            var expected = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                var count = i;
                expected.Add(count);

                BackgroundQueue.QueueBackgroundWorkItem(async token =>
                {
                    // Simulate a Background Task
                    await Task.Delay(TimeSpan.FromMilliseconds(20), token);
                    generated.Add(count);
                });
            }
            
            while (BackgroundQueue.ItemCount > 0)
            {
                var work = await BackgroundQueue.DequeueAsync(CancellationToken.None);
                await work(CancellationToken.None);
            }

            generated.Should().Equal(expected);
        }
    }
}
