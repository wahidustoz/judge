using Ilmhub.Judge.Messaging.Shared;
using Ilmhub.Judge.Messaging.Shared.Converters;
using Ilmhub.Judge.Messaging.Shared.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Sdk;

public class JudgeSdkBuilder
{
    private readonly IServiceCollection services;

    public JudgeMessagingSettings Messaging { get; set; }

    public JudgeSdkBuilder(IServiceCollection services, JudgeMessagingSettings messaging)
    {
        this.services = services;
        Messaging = messaging;
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

    private void AddMasstransitJudgeEventHandlers(Action<IReceiveEndpointConfigurator, IBusRegistrationContext> configurator)
    {
        services.AddMassTransit<IJudgeEventsBus>(x =>
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