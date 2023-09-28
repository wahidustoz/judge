using System.Threading.RateLimiting;
using HealthChecks.UI.Client;
using Ilmhub.Judge.Api.Jaeger;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
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
                    .AddHttpClientInstrumentation(o =>
                    {
                        // list of excluded hosts from http client telemetry.
                        var excludedHosts = new List<string>() { };
                        o.FilterHttpWebRequest = filter => !excludedHosts.Contains(filter.RequestUri.Host);
                    })
                    .AddSource("Ilmhub.Judge.Sdk")
                    .AddSource("Ilmhub.Judge.Api")
                    .AddSource("Ilmhub.Judge.Wrapper")
                    .AddConsoleExporter(c => c.Targets = ConsoleExporterOutputTargets.Debug);

                if (configuration["OpenTelemetry:Driver"] == "Jaeger")
                {
                    builder.AddOtlpExporter();
                }
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

    public static IServiceCollection ConfigureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("RateLimiting:Enabled") == false)
            return services;

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = configuration.GetValue<int>("RateLimiting:Permit");
                options.Window = TimeSpan.FromSeconds(configuration.GetValue<int>("RateLimiting:Window"));
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = configuration.GetValue<int>("RateLimiting:QueueLimit");
            });
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        return services;
    }
}