using Ilmhub.Judge.Client.Abstractions;
using Ilmhub.Judge.Client.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Client.Tests.Integration;

public class PingTest
{
    private readonly ServiceProvider provider;

    public PingTest()
    {
        var configuration = ConfigurationMockFactory.Create(new Dictionary<string, string>
        {
            { "JudgeServer:BaseUrl", "http://localhost:12358" },
            { "JudgeServer:Token", "dccf62d6-3628-4ad0-a30e-49d159e54136" }
        });
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddJudgeServerClient(configuration);
        provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task ValidPingSucceedsAsync()
    {
        var configuration = ConfigurationMockFactory.Create(new Dictionary<string, string>
        {
            { "JudgeServer:BaseUrl", "http://localhost:12358" },
            { "JudgeServer:Token", "dccf62d6-3628-4ad0-a30e-49d159e54136" }
        });
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddJudgeServerClient(configuration);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var response = await client.PingAsync();

        Assert.NotNull(response);
    }

    [Fact]
    public async Task PingWithInvalidTokenFailsAsync()
    {
        var configuration = ConfigurationMockFactory.Create(new Dictionary<string, string>
        {
            { "JudgeServer:BaseUrl", "http://localhost:12358" },
            { "JudgeServer:Token", "POTATO" }
        });
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddJudgeServerClient(configuration);
        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IJudgeServerClient>();

        var pingTask = () => client.PingAsync().AsTask();
        
        await Assert.ThrowsAsync<PingFailedException>(pingTask);
    }
}
