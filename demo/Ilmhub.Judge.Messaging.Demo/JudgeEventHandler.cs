using Ilmhub.Judge.Messaging.Demo;
using Ilmhub.Judge.Messaging.Shared.Events;
using Ilmhub.Judge.Sdk.Messaging;
using Microsoft.Extensions.Logging;

public class JudgeEventHandler : IJudgeEventHandler
{
    private readonly ILogger<JudgeEventHandler> logger;

    public JudgeEventHandler(ILogger<JudgeEventHandler> logger)
    {
        this.logger = logger;
    }
    public ValueTask HandleJudgeCompletedAsync(JudgeCompleted @event, CancellationToken cancellationToken = default)
    {
        logger.LogJudgeEvent(@event.GetType().Name, @event);
        return ValueTask.CompletedTask;
    }

    public ValueTask HandleJudgeFailedAsync(JudgeFailed @event, CancellationToken cancellationToken = default)
    {
        logger.LogJudgeEvent(@event.GetType().Name, @event);
        return ValueTask.CompletedTask;
    }

    public ValueTask HandleRunCompletedAsync(RunCompleted @event, CancellationToken cancellationToken = default)
    {
        logger.LogJudgeEvent(@event.GetType().Name, @event);
        return ValueTask.CompletedTask;
    }

    public ValueTask HandleRunFailedAsync(RunFailed @event, CancellationToken cancellationToken = default)
    {
        logger.LogJudgeEvent(@event.GetType().Name, @event);
        return ValueTask.CompletedTask;
    }
}