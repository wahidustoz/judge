using System.Threading.RateLimiting;
using FluentValidation;
using HealthChecks.UI.Client;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Api.Jaeger;
using Ilmhub.Judge.Api.Validators;
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
            options.AddLogRecordProcessor();
            options.AddConsoleExporter(c => c.Targets = ConsoleExporterOutputTargets.Debug);
            options.AddOtlpExporter(o =>
            {
                var jaegerEndpoint = Environment.GetEnvironmentVariable("JAEGER_ENDPOINT");
                if (string.IsNullOrWhiteSpace(jaegerEndpoint) is false)
                    o.Endpoint = new Uri(jaegerEndpoint);
            });
        });
        return builder;
    }

    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue("OpenTelemetry:Driver", "None") == "None")
            return services;

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.SetSampler(new AlwaysOnSampler());

                builder
                    .AddSource(configuration["OpenTelemetry:ServiceName"])
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(configuration["OpenTelemetry:ServiceName"]))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter(c => c.Targets = ConsoleExporterOutputTargets.Debug)
                    .AddOtlpExporter(options =>
                    {
                        var jaegerEndpoint = Environment.GetEnvironmentVariable("JAEGER_ENDPOINT");
                        if (string.IsNullOrWhiteSpace(jaegerEndpoint) is false)
                            options.Endpoint = new Uri(jaegerEndpoint);
                    });
            });

        return services;
    }

    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = configuration.GetValue("RateLimiting:Permit", 1);
                options.Window = TimeSpan.FromSeconds(configuration.GetValue("RateLimiting:Window", 1));
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = configuration.GetValue("RateLimiting:QueueLimit", 1);
            });
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });
        return services;
    }

    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<JudgeRequestDto>, JudgeRequestValidator>();
        services.AddTransient<IValidator<IFormFile>, TestCaseFormFileValidator>();
        services.AddTransient<IValidator<IEnumerable<TestCaseDto>>, TestCaseRequestValidator>();

        return services;
    }
}

public static class WebApplicationExtensions
{
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