using HealthChecks.UI.Client;
using Ilmhub.Judge.Api.Jaeger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ilmhub.Judge.Api;

public static class ServiceCollectionExtensions
{
    public static ILoggingBuilder ConfigureOpenTelemetryLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        if (configuration.GetValue("OpenTelemetry:Driver", "None") == "None")
            return builder;
        
        builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;

            options.AddConsoleExporter();
            options.AddLogRecordProcessor();
        });
        return builder;
    }

    public static IServiceCollection SetupOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue("OpenTelemetry:Driver", "None") == "None")
            return services;

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.SetSampler(new AlwaysOnSampler());

                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["OpenTelemetry:ServiceName"]))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter(c => c.Targets = ConsoleExporterOutputTargets.Debug);

                if (configuration["OpenTelemetry:Driver"] == "Jaeger")
                    builder.AddOtlpExporter();
            });

        return services;
    }

    public static IServiceCollection SetupHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<CompilerHealthChecks>("Compilers");
        return services;
    }

    public static WebApplication ConfigureHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions { Predicate = _ => true });
        app.UseHealthChecks("/healthz", new HealthCheckOptions 
        { 
            Predicate = _ => true, 
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse 
        });

        return app;
    }
}