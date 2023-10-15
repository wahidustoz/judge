using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJudgeMessaging(this IServiceCollection services, IConfiguration configuration, Action<JudgeMessagingOptionsBuilder> optionsBuilder)
    {
        services.AddTransient<IJudgeEventPublisher, JudgeEventPublisher>();
        
        var settings = new JudgeMessagingSettings();
        configuration.GetSection("Messaging").Bind(settings);
        services.AddSingleton(settings);

        var builder = new JudgeMessagingOptionsBuilder(services, settings);
        optionsBuilder(builder);
        
        return services;
    }
}
