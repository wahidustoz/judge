using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Api.Logging;
using Ilmhub.Judge.Messaging;
using Ilmhub.Judge.Messaging.Exceptions;
using Ilmhub.Judge.Messaging.Shared.Commands;
using Ilmhub.Judge.Messaging.Shared.Events;

namespace Ilmhub.Judge.Api;

public class JudgeCommandHandler : ICommandHandler<JudgeCommand>
{
    private readonly IJudgeEventPublisher publisher;
    private readonly ILogger<JudgeCommandHandler> logger;
    private readonly IJudger judger;

    public JudgeCommandHandler(
        IJudgeEventPublisher publisher,
        ILogger<JudgeCommandHandler> logger,
        IJudger judger)
    {
        this.publisher = publisher;
        this.logger = logger;
        this.judger = judger;
    }

    public async ValueTask HandleAsync(JudgeCommand command, CancellationToken cancellationToken)
    {
        logger.LogCommandHandlerStarted(nameof(JudgeCommand), command.RequestId);
        try
        {
            var result = await judger.JudgeAsync(
                languageId: command.LanguageId,
                source: command.SourceCode,
                testCaseId: command.TestCaseId,
                useStrictMode: command.UseStrictMode,
                maxCpu: command.MaxCpu ?? -1,
                maxMemory: command.MaxMemory ?? -1,
                cancellationToken: cancellationToken);

            await publisher.PublishAsync(new JudgeCompleted
            {
                RequestId = command.RequestId,
                SourceId = command.SourceId,
                SourceContext = command.SourceContext,
                Source = command.Source,
                Status = result.Status.ToString(),
                CompilationResult = new CompilationResult
                {
                    IsSuccess = result.Compilation.IsSuccess,
                    Status = result.Compilation.Execution.Status.ToString(),
                    Output = result.Compilation.Output,
                    Error = result.Compilation.Error
                },
                TestCases = result.TestCases?.Select(t => new TestCaseResult
                {
                    Id = t.Id,
                    Status = t.Status.ToString(),
                    Output = t.Output,
                    OutputMd5 = t.OutputMd5,
                    CpuTime = t.Execution.Execution.CpuTime,
                    RealTime = t.Execution.Execution.RealTime,
                    Memory = t.Execution.Execution.Memory
                })
            }, cancellationToken);
        }
        // We do NOT catch FailedToPublishJudgeEventException because we don't want to handle it
        catch (Exception ex) when (ex is not FailedToPublishJudgeEventException)
        {
            logger.LogJudgeFailedException(ex, command.RequestId);
            await publisher.PublishAsync(new JudgeFailed
            {
                RequestId = command.RequestId,
                SourceId = command.SourceId,
                SourceContext = command.SourceContext,
                Source = command.Source,
                Error = ex.Message
            }, cancellationToken);
        }
        logger.LogCommandHandlerCompleted(nameof(JudgeCommand), command.RequestId);
    }
}