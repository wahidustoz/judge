using Ilmhub.Judge.Messaging.Shared.Events;

namespace Ilmhub.Judge.Sdk.Messaging;

public interface IJudgeEventHandler
{
    ValueTask HandleJudgeCompletedAsync(JudgeCompleted @event, CancellationToken cancellationToken = default);
    ValueTask HandleJudgeFailedAsync(JudgeFailed @event, CancellationToken cancellationToken = default);
    ValueTask HandleRunCompletedAsync(RunCompleted @event, CancellationToken cancellationToken = default);
    ValueTask HandleRunFailedAsync(RunFailed @event, CancellationToken cancellationToken = default);
}