using Ilmhub.Judge.Messaging.Shared;
using Ilmhub.Judge.Messaging.Shared.Interfaces;
using Ilmhub.Judge.Sdk.Exceptions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Ilmhub.Judge.Sdk;

public class JudgeCommandPublisher : IJudgeCommandPublisher {
    private readonly IJudgeEventsBus bus;
    private readonly ILogger<JudgeCommandPublisher> logger;

    public JudgeCommandPublisher(
        IJudgeEventsBus bus,
        ILogger<JudgeCommandPublisher> logger) {
        this.bus = bus;
        this.logger = logger;
    }

    public async ValueTask PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
        => await GetRetryPolicy().ExecuteAsync(async ()
            => await ExecutePublishAsync(command, cancellationToken));

    public async ValueTask ScheduleCommandAsync<TCommand>(TCommand command, TimeSpan delay, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
        => await GetRetryPolicy().ExecuteAsync(async ()
            => await ExecuteScheduleAsync(command, delay, cancellationToken));

    private async ValueTask ExecuteScheduleAsync<TCommand>(TCommand command, TimeSpan delay, CancellationToken cancellationToken)
        where TCommand : class, ICommand {
        try {
            var endpoint = new Uri($"queue:{Queues.JudgeOperations}");
            var scheduler = bus.CreateDelayedMessageScheduler();
            var when = DateTime.UtcNow.Add(delay);
            await scheduler.ScheduleSend(endpoint, when, command, cancellationToken);
        }
        catch (Exception ex) {
            logger.LogWarning(ex, "Failed to send Judge command for command type {type}", typeof(TCommand).Name);
            throw new FailedToPublishJudgeCommandException(ex);
        }
    }

    public async ValueTask ExecutePublishAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand {
        try {
            var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{Queues.JudgeOperations}"));
            await endpoint.Send(command, cancellationToken);
        }
        catch (Exception ex) {
            logger.LogWarning(ex, "Failed to send Judge command for command type {type}", typeof(TCommand).Name);
            throw new FailedToPublishJudgeCommandException(ex);
        }
    }

    private AsyncRetryPolicy GetRetryPolicy() => Policy
        .Handle<FailedToPublishJudgeCommandException>()
        .WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        },
        onRetry: (exception, timeSince, retryCount, ctx) => {
            logger.LogInformation(
                exception,
                "Retrying to send message to queue {queue} count: {retryCount}",
                Queues.JudgeOperations,
                retryCount);
        });
}