using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Sdk.Tests.Integration;

public class PingTest
{
    private readonly ServiceProvider provider;

    public PingTest()
        => provider = ServiceCollectionMockProvider.SetupServiceProvider("http://localhost:12358", "123token");

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
        var provider = ServiceCollectionMockProvider.SetupServiceProvider("http://localhost:12358", "POTATO");
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var pingTask = () => client.PingAsync().AsTask();

        await Assert.ThrowsAsync<PingFailedException>(pingTask);
    }

    [Fact]
    public async Task PingWithInvalidUrlThrowsHttpRequestExceptionAsync()
    {
        var provider = ServiceCollectionMockProvider.SetupServiceProvider("http://localhost:123", "POTATO");
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var pingTask = () => client.PingAsync().AsTask();

        await Assert.ThrowsAsync<HttpRequestException>(pingTask);
    }
}
