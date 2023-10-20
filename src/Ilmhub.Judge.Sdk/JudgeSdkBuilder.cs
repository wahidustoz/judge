using Ilmhub.Judge.Messaging.Shared;
using Ilmhub.Judge.Messaging.Shared.Converters;
using Ilmhub.Judge.Messaging.Shared.Events;
using Ilmhub.Judge.Sdk.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Telemetry;

namespace Ilmhub.Judge.Sdk;

public class JudgeSdkBuilder
{
    private readonly IServiceCollection services;
    private readonly JudgeSdkSettings settings;
    public JudgeMessagingSettings Messaging => settings.Messaging;

    public JudgeSdkBuilder(IServiceCollection services, JudgeSdkSettings settings)
    {
        this.services = services;
        this.settings = settings;
    }

    public JudgeSdkBuilder AddCommandPublisher()
    {
        services.AddTransient<IJudgeCommandPublisher, JudgeCommandPublisher>();
        return this;
    }

    public JudgeSdkBuilder AddJudgeClient()
    {
        // TODO: Add judge client
        services.AddHttpClient<IJudgeClient, JudgeClient>(b => b.BaseAddress = new Uri(settings.Endpoint));
        services.AddResiliencePipeline(nameof(JudgeClient), (options, context) =>
        {
            options.ConfigureTelemetry(new TelemetryOptions { });
            options.AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>(ex => ex.IsClientError() is false),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                OnRetry = args =>
                {
                    var logger = context.ServiceProvider.GetRequiredService<ILogger<JudgeClient>>();
                    logger.LogTrace(
                        args.Outcome.Exception, 
                        "Retrying JudgeClient error for attempt: {attemptNumber}",
                        args.AttemptNumber);
                    return ValueTask.CompletedTask;
                }
            });
        });

        return this;
    }

    public JudgeSdkBuilder AddJudgeEventHandler<TJudgeEventHandler>(TJudgeEventHandler eventHandler)
        where TJudgeEventHandler : class, IJudgeEventHandler
    {
        services.AddSingleton<IJudgeEventHandler>(eventHandler);
        RegisterSingleHandler();
        return this;
    }

    public JudgeSdkBuilder AddJudgeEventHandler<TJudgeEventHandler>()
        where TJudgeEventHandler : class, IJudgeEventHandler
    {
        services.AddTransient<IJudgeEventHandler, TJudgeEventHandler>();
        RegisterSingleHandler();
        return this;
    }

    private void RegisterSingleHandler()
    {
        AddMasstransitJudgeEventHandlers((receiver, busContext) =>
        {
            receiver.Handler<JudgeCompleted>(async context =>
            {
                using var scope = busContext.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IJudgeEventHandler>();
                await service.HandleJudgeCompletedAsync(context.Message, context.CancellationToken);
            });
            receiver.Handler<JudgeFailed>(async context =>
            {
                using var scope = busContext.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IJudgeEventHandler>();
                await service.HandleJudgeFailedAsync(context.Message, context.CancellationToken);
            });
            receiver.Handler<RunCompleted>(async context =>
            {
                using var scope = busContext.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IJudgeEventHandler>();
                await service.HandleRunCompletedAsync(context.Message, context.CancellationToken);
            });
            receiver.Handler<RunFailed>(async context =>
            {
                using var scope = busContext.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IJudgeEventHandler>();
                await service.HandleRunFailedAsync(context.Message, context.CancellationToken);
            });
        });
    }

    private IServiceCollection AddMasstransitJudgeEventHandlers(Action<IReceiveEndpointConfigurator, IBusRegistrationContext> configurator)
    {
        return services.AddMassTransit<IJudgeEventsBus>(x =>
        {
            if (Messaging.Driver == "RabbitMQ")
            {
                var host = Messaging.RabbitMQ.Host;
                var username = Messaging.RabbitMQ.Username;
                var password = Messaging.RabbitMQ.Password;

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureJsonSerializerOptions(o => o.AddJudgeMessageSerializationOptions());
                    cfg.Host(host, config =>
                    {
                        config.Username(username);
                        config.Password(password);
                    });

                    cfg.ReceiveEndpoint(Queues.JudgeEvents, (e) =>
                    {
                        e.Bind(Queues.JudgeEvents);
                        configurator.Invoke(e, context);
                    });
                });
            }
            else
                throw new NotSupportedException("Unsupported messaging driver: " + Messaging.Driver);
        });
    }
}