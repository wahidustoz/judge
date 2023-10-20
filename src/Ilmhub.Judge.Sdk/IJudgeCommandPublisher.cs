using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Sdk;

public interface IJudgeCommandPublisher {
    ValueTask PublishCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
    ValueTask ScheduleCommandAsync<TCommand>(TCommand command, TimeSpan delay, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}