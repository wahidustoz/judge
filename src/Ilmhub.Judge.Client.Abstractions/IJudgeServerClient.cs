using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Abstractions;

public interface IJudgeServerClient
{
    ValueTask<IServerInfo> PingAsync(CancellationToken cancellationToken = default);
    ValueTask<IJudgeResult> JudgeAsync(IJudgeRequest request, CancellationToken cancellationToken = default);
}