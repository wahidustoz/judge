using Ilmhub.Judge.Client.Abstractions;
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
    public async Task PingAsync()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var response = await client.PingAsync();
        Assert.NotEmpty(response);
    }
}
