using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Ilmhub.Judge.Api;

public static class ServiceCollectionConfgiureRateLimiting
{
    public static IServiceCollection ConfgiureRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        if(configuration.GetValue<bool>("RateLimitingOptions:EnableRateLimiting") == false)
            return services;

        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", options =>
            {
                options.PermitLimit = configuration.GetValue<int>("RateLimitingOptions:Permit");
                options.Window = configuration.GetValue<TimeSpan>("RateLimitingOptions:Window");
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = configuration.GetValue<int>("RateLimitingOptions:QueueLimit");
            });
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsync(
                "Too many requests. Please try again later.", cancellationToken: token);
            };
        });
        return services;
    }
}