using System.Net.Http.Json;
using Ilmhub.Judge.Client.Abstractions;

namespace Ilmhub.Judge.Client;

public class JudgeServerClient : IJudgeServerClient
{
    private readonly HttpClient client;

    public JudgeServerClient(HttpClient client) => this.client = client;

    public async ValueTask<string> PingAsync(CancellationToken cancellationToken = default)
    {
        var repsonse = await client.PostAsJsonAsync("/ping", new { }, cancellationToken);
        return await repsonse.Content.ReadAsStringAsync(cancellationToken);
    }
}
