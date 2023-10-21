using Ilmhub.Judge.Api.Filters;

namespace Ilmhub.Judge.Api;

public static class EndpointConventionBuilderExtensions
{
    public static IEndpointConventionBuilder WithRateLimiting(this IEndpointConventionBuilder builder, string policy, IConfiguration configuration)
    {
        if(configuration.GetValue("RateLimiting:Enabled", false))
            builder.RequireRateLimiting(policy);
        return builder;
    }

    public static IEndpointConventionBuilder WithAsyncValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        builder.AddEndpointFilter<AsyncFluentValidationFilter<T>>();
        return builder;
    }
}