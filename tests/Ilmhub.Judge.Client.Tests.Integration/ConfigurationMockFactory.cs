using Microsoft.Extensions.Configuration;

namespace Ilmhub.Judge.Sdk.Tests.Integration;

internal class ConfigurationMockFactory
{
    public static IConfiguration Create(Dictionary<string, string> keyValues)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(keyValues)
            .Build();

        return configuration;
    }
}
