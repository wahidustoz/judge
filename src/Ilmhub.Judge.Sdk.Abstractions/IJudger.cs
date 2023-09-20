using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudger
{
    ValueTask<IJudgeResult> JudgeAsync(IJudgeRequest request, CancellationToken cancellationToken = default);
}
