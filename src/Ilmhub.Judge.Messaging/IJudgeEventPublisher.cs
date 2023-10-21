using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging;

public interface IJudgeEventPublisher
{
    ValueTask PublishAsync<TEvent>(TEvent judgeEvent, CancellationToken cancellationToken)
        where TEvent : class, IJudgeEvent;
}