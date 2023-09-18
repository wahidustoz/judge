using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface IJudgeServerClient
{
    ValueTask<IServerInfo> PingAsync(CancellationToken cancellationToken = default);
    ValueTask<IJudgeResult> JudgeAsync(IJudgeRequest request, CancellationToken cancellationToken = default);
    ValueTask CompileSpecialAsync(ICompileSpecialRequest request, CancellationToken cancellationToken = default);
    ValueTask<IJudgeResult> JudgeSpecialAsync(IJudgeSpecialRequest request, CancellationToken cancellationToken = default);
}