using Ilmhub.Judge.Api.Logging;
using Ilmhub.Judge.Messaging;
using Ilmhub.Judge.Messaging.Shared.Commands;
using Ilmhub.Judge.Messaging.Shared.Events;

namespace Ilmhub.Judge.Api;
public class RunCommandHandler : ICommandHandler<RunCommand>
{
    private readonly IJudgeEventPublisher publisher;
    private readonly ILogger<RunCommandHandler> logger;

    public RunCommandHandler(IJudgeEventPublisher publisher, ILogger<RunCommandHandler> logger)
    {
        this.publisher = publisher;
        this.logger = logger;
    }

    public async ValueTask HandleAsync(RunCommand command, CancellationToken cancellationToken)
    {
        logger.LogCommandHandlerStarted(typeof(RunCommand).Name, command.RequestId);
        await publisher.PublishAsync(new RunFailed
        {
            RequestId = command.RequestId,
            SourceId = command.SourceId,
            SourceContext = command.SourceContext,
            Source = command.Source,
            Error = "Run command not implemented yet."
        }, cancellationToken);
        logger.LogCommandHandlerCompleted(typeof(RunCommand).Name, command.RequestId);
    }
}
