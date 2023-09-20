using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk;

public class Judger : IJudger
{
    public ValueTask<IJudgeResult> JudgeAsync(IJudgeRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
