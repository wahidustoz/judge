using Ilmhub.Judge.Wrapper.Abstractions.Models;

namespace Ilmhub.Judge.Wrapper.Abstractions;

public interface IJudgeWrapper {
    ValueTask<IExecutionResult> ExecuteJudgerAsync(IExecutionRequest request, CancellationToken cancellationToken = default);
}