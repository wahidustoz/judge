using Ilmhub.Judge.Messaging.Shared;
using Ilmhub.Judge.Messaging.Shared.Converters;
using Ilmhub.Judge.Messaging.Shared.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Messaging;

public class JudgeMessagingOptionsBuilder {
    private readonly IServiceCollection services;
    private readonly JudgeMessagingSettings settings;

    public JudgeMessagingOptionsBuilder(IServiceCollection services, JudgeMessagingSettings settings) {
        this.services = services;
        this.settings = settings;
    }

    public JudgeMessagingOptionsBuilder AddMasstransitBus(Action<IReceiveEndpointConfigurator, IServiceProvider> receiverConfigurator) {
        if (settings.Driver is "RabbitMQ") {
            services.AddMassTransit(x => {
                x.AddDelayedMessageScheduler();
                x.UsingRabbitMq((context, cfg) => {

                    cfg.UseDelayedMessageScheduler();
                    cfg.ConfigureJsonSerializerOptions(o => o.AddJudgeMessageSerializationOptions());
                    cfg.Host(settings.RabbitMQ.Host, config => {
                        config.Username(settings.RabbitMQ.Username);
                        config.Password(settings.RabbitMQ.Password);
                    });

                    cfg.ReceiveEndpoint($"{Queues.JudgeOperations}", e => {
                        receiverConfigurator(e, context);

                        if (e is IRabbitMqReceiveEndpointConfigurator rabbitMqReceiveEndpointConfigurator)
                            ConfigureRabbitMQReceiveEndpoint(rabbitMqReceiveEndpointConfigurator);
                        else
                            throw new NotSupportedException("Unsupported receive endpoint configurator: " + e.GetType().Name);
                    });
                });
            });
        }
        else
            throw new NotSupportedException("Unsupported messaging driver: " + settings.Driver);

        return this;
    }

    public JudgeMessagingOptionsBuilder RegisterCommandHandler<TCommand, THandler>()
        where THandler : class, ICommandHandler<TCommand>
        where TCommand : class, ICommand {
        services.AddTransient<ICommandHandler<TCommand>, THandler>();
        return this;
    }

    private static void ConfigureRabbitMQReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator config) {
        config.UseScheduledRedelivery(r => r.Immediate(5));
        config.UseMessageRetry(r => r.Immediate(5));
    }
}