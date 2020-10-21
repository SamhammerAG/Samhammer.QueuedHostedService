using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Samhammer.QueuedHostedService.Test
{
    public class QueuedHostedServiceTest
    {
        private IBackgroundTaskQueue BackgroundQueue { get; }

        private IHostedService HostedService { get; }

        public QueuedHostedServiceTest()
        {
            var logger = new NullLogger<QueuedHostedService>();

            var services = new ServiceCollection();
            services.AddBackgroundQueue();
            services.AddSingleton<ILogger<QueuedHostedService>>(logger);

            var serviceProvider = services.BuildServiceProvider();

            BackgroundQueue = serviceProvider.GetService<IBackgroundTaskQueue>();
            HostedService = serviceProvider.GetService<IHostedService>();
        }

        [Fact]
        public async Task QueueTasksBeforeRunning()
        {
            // Arrange
            var generated = new List<int>();
            var expected = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                expected.Add(i);
            }

            // Act
            for (var i = 0; i < 100; i++)
            {
                var count = i;

                BackgroundQueue.Enqueue(token =>
                {
                    generated.Add(count);
                    return Task.CompletedTask;
                });
            }

            await HostedService.StartAsync(CancellationToken.None);
            await Delay(1000, new CancellationTokenSource());
            await HostedService.StopAsync(CancellationToken.None);

            // Assert all tasks are executed in correct order
            generated.Should().Equal(expected);
        }

        [Fact]
        public async Task QueueTasksWhileRunning()
        {
            // Arrange
            var generated = new List<int>();
            var expected = new List<int>();

            for (var i = 0; i < 100; i++)
            {
                expected.Add(i);
            }

            // Act
            await HostedService.StartAsync(CancellationToken.None);

            for (var i = 0; i < 100; i++)
            {
                var count = i;

                BackgroundQueue.Enqueue(token =>
                {
                    generated.Add(count);
                    return Task.CompletedTask;
                });
            }

            await Delay(1000, new CancellationTokenSource());
            await HostedService.StopAsync(CancellationToken.None);

            // Assert all tasks are executed in correct order
            generated.Should().Equal(expected);
        }

        [Fact]
        public async Task ContinueRunningAfterFailedTask()
        {
            // Arrange
            var generated = new List<int>();
            var expected = new List<int> { 1, 3 };

            // Act
            await HostedService.StartAsync(CancellationToken.None);

            BackgroundQueue.Enqueue(token =>
            {
                generated.Add(1);
                return Task.CompletedTask;
            });
            BackgroundQueue.Enqueue(token => throw new Exception("Other tasks should execute properly"));
            BackgroundQueue.Enqueue(token =>
            {
                generated.Add(3);
                return Task.CompletedTask;
            });

            await Delay(1000, new CancellationTokenSource());
            await HostedService.StopAsync(CancellationToken.None);

            // Assert tasks execute when another task fails
            generated.Should().Equal(expected);
        }

        private async Task Delay(int milliseconds, CancellationTokenSource cancelSource)
        {
            try
            {
                await Task.Delay(milliseconds, cancelSource.Token);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
