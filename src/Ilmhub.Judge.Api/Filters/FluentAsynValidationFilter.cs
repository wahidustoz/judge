using System.Net;
using FluentValidation;

namespace Ilmhub.Judge.Api.Filters;

public class FluentAsynValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly ILogger<FluentAsynValidationFilter<T>> logger;

    public FluentAsynValidationFilter(ILogger<FluentAsynValidationFilter<T>> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogInformation("Executing FluentAsynValidationFilter...");
        var argumentOrNull = context.Arguments.FirstOrDefault(a => a is T);
        IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if(argumentOrNull is T argumentToValidate is false)
        {
            logger.LogError("Request is missing required object.");
            return Results.BadRequest("Request is missing required object.");
        }      

        if(validator is not null)
        {
            var validationResult = await validator.ValidateAsync(argumentToValidate);
            if(validationResult.IsValid is false)
            {
                logger.LogError("Validation failed: {Errors}", validationResult.Errors);
                return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }
        }

        var result = await next.Invoke(context);
        logger.LogInformation("FluentAsynValidationFilter executed successfully.");
        return result;
    }
}