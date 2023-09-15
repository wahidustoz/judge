using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Client.Tests.Integration;

internal class ServiceCollectionMockProvider
{
    public static ServiceProvider SetupServiceProvider(string baseUrl, string token)
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