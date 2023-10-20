using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand {
    ValueTask HandleAsync(TCommand command, CancellationToken cancellationToken);
}