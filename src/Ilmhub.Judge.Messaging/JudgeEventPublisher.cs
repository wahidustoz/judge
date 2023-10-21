using System.Text.Json;
using Ilmhub.Judge.Messaging.Exceptions;
using Ilmhub.Judge.Messaging.Shared;
using Ilmhub.Judge.Messaging.Shared.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Ilmhub.Judge.Messaging;

public class JudgeEventPublisher : IJudgeEventPublisher
{
    private readonly ILogger<JudgeEventPublisher> logger;
    private readonly IBus bus;

    public JudgeEventPublisher(
        ILogger<JudgeEventPublisher> logger,
        IBus bus)
    {
        this.logger = logger;
        this.bus = bus;
    }

    public async ValueTask PublishAsync<TEvent>(TEvent judgeEvent, CancellationToken cancellationToken)
        where TEvent : class, IJudgeEvent
        => await GetRetryPolicy().ExecuteAsync(async ()
            => await ExecutePublishAsync(judgeEvent, cancellationToken));

    private async ValueTask ExecutePublishAsync<TEvent>(TEvent judgeEvent, CancellationToken cancellationToken)
        where TEvent : class, IJudgeEvent
    {
        try
        {
            string prefix = null;
            if (bus.Topology is IRabbitMqBusTopology)
                prefix = "exchange";
            else
                prefix = "topic";

            string source = null;
            if (judgeEvent is IHasSource eventWithSource)
                source = eventWithSource.Source;

            var endpoint = await bus.GetSendEndpoint(new Uri($"{prefix}:{Queues.JudgeEvents}"));
            await endpoint.Send(judgeEvent, context => context.Headers.Set("source", source), cancellationToken);
        }
        catch (Exception ex) when (ex is not JsonException)
        {
            throw new FailedToPublishJudgeEventException(judgeEvent.GetType(), ex);
        }
    }

    private AsyncRetryPolicy GetRetryPolicy() => Policy
        .Handle<FailedToPublishJudgeEventException>()
        .WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10)
        },
        onRetry: (exception, timeSince, retryCount, ctx) =>
        {
            logger.LogInformation(
                exception,
                "Retrying to send message to queue {queue} count: {retryCount}",
                Queues.JudgeEvents,
                retryCount);
        });
}