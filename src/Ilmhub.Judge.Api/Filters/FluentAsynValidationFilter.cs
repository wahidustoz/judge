using System.Net;
using FluentValidation;
using Ilmhub.Judge.Api.Logging;

namespace Ilmhub.Judge.Api.Filters;

public class AsyncFluentValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly ILogger<AsyncFluentValidationFilter<T>> logger;

    public AsyncFluentValidationFilter(ILogger<AsyncFluentValidationFilter<T>> logger)
        => this.logger = logger;

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            var argumentOrNull = context.Arguments.FirstOrDefault(a => a is T);
            IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (argumentOrNull is T argumentToValidate is false)
                return Results.BadRequest("Request is missing required object.");

            logger.LogValidationStarted(nameof(T));
            if (validator is not null)
            {
                var validationResult = await validator.ValidateAsync(argumentToValidate);
                if (validationResult.IsValid is false)
                {
                    logger.LogValidationFailed(
                        targetType: nameof(T),
                        errors: validationResult.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage));
                    return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.UnprocessableEntity);
                }
            }
            logger.LogValidationCompleted(nameof(T));
            return await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogValidationException(nameof(T), ex);
            return Results.Problem(ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}