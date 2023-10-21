using System.Diagnostics;
using Ilmhub.Judge.Messaging.Shared.Interfaces;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Messaging;

public static class MasstransitExtensions
{
    public static IReceiveEndpointConfigurator RegisterConsumer<TCommand>(this IReceiveEndpointConfigurator e, IServiceProvider serviceProvider)
        where TCommand : class, ICommand
    {
        e.Handler<TCommand>(async commandContext =>
        {
            using var scope = serviceProvider.CreateScope();
            var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

            var activitySource = new ActivitySource("Ilmhub.Judge.Messaging");
            using var activity = activitySource.StartActivity(typeof(TCommand).Name);
            await commandHandler.HandleAsync(commandContext.Message, commandContext.CancellationToken);
        });
        return e;
    }
}