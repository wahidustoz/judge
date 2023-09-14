namespace Ilmhub.Judge.Client.Abstractions;

public interface IJudgeServerClient
{
    ValueTask<string> PingAsync(CancellationToken cancellationToken = default);
}
