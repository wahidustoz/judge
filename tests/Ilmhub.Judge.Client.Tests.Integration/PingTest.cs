using Ilmhub.Judge.Client.Abstractions;
using Ilmhub.Judge.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Client.Tests.Integration;

public class PingTest
{
    private readonly ServiceProvider provider;

    public PingTest()
        => provider = SetupServiceProvider("http://localhost:12358", "123token");

    [Fact]
    public async Task ValidPingSucceedsAsync()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var response = await client.PingAsync();

        Assert.NotNull(response);
    }

    [Fact]
    public async Task PingWithInvalidTokenThrowsPingFailedExceptionAsync()
    {
        var provider = SetupServiceProvider("http://localhost:12358", "POTATO");
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var pingTask = () => client.PingAsync().AsTask();

        await Assert.ThrowsAsync<PingFailedException>(pingTask);
    }

    [Fact]
    public async Task PingWithInvalidUrlThrowsHttpRequestExceptionAsync()
    {
        var provider = SetupServiceProvider("http://localhost:123", "POTATO");
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var pingTask = () => client.PingAsync().AsTask();

        await Assert.ThrowsAsync<HttpRequestException>(pingTask);
    }

    private ServiceProvider SetupServiceProvider(string baseUrl, string token)
    {
        var configuration = ConfigurationMockFactory.Create(new Dictionary<string, string>
            {
                { "JudgeServer:BaseUrl", baseUrl },
                { "JudgeServer:Token", token }
            });

        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddJudgeServerClient(configuration);

        return services.BuildServiceProvider();
    }
}
