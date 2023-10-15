﻿using System.Data;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Sdk.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIlmhubJudge(
        this IServiceCollection services, 
        Action<JudgeSdkBuilder> configurator = null)
    {
        services.AddTransient<IJudgeCommandPublisher, JudgeCommandPublisher>();
        
        var settings = new JudgeSettings();
        var builder = new JudgeSdkBuilder(services, settings);
        configurator?.Invoke(builder);

        return services;
    }
}
