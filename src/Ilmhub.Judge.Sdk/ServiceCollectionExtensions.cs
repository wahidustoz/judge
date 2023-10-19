using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIlmhubJudge(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JudgeSdkBuilder> configurator = null)
    {
        var settings = new JudgeSdkSettings();
        configuration.GetSection("Judge").Bind(settings);
        var builder = new JudgeSdkBuilder(services, settings);
        configurator?.Invoke(builder);

        return services;
    }
}
