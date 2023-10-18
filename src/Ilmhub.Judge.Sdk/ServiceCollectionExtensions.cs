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
        services.AddTransient<IJudgeCommandPublisher, JudgeCommandPublisher>();

        var messaging = new JudgeMessagingSettings();
        configuration.GetSection("Messaging").Bind(messaging);
        var builder = new JudgeSdkBuilder(services, messaging);
        configurator?.Invoke(builder);

        return services;
    }
}
