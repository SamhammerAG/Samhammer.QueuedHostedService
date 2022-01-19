[![Build Status](https://travis-ci.com/SamhammerAG/Samhammer.QueuedHostedService.svg?branch=master)](https://travis-ci.com/SamhammerAG/Samhammer.QueuedHostedService)

## Samhammer.QueuedHostedService

This package provides a hosted queue service to execute background tasks in order.

The implementation is based on this documentation:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio#queued-background-tasks

#### How to add this to your project:
- reference this nuget package: https://www.nuget.org/packages/Samhammer.QueuedHostedService/

#### How to use:

Register the background queue service in startup:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddBackgroundQueue();
}
```

Add tasks to the queue:
```csharp
public class SampleService
{
    private IBackgroundTaskQueue BackgroundQueue { get; }
	
    public SampleService(IBackgroundTaskQueue backgroundQueue)
    {
        BackgroundQueue = backgroundQueue;
    }
	
    public SampleMethod() {
        BackgroundQueue.Enqueue(cancellationToken =>
        {
            Console.WriteLine("Executing background task.");
            return Task.CompletedTask;
        });
    }
}
```

## Contribute

#### How to publish a nuget package
- Create a tag and let the github action do the publishing for you