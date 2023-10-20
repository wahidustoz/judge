using System.Net;
using FluentValidation;
using Ilmhub.Judge.Api.Logging;

namespace Ilmhub.Judge.Api.Filters;

public class FluentAsynValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly ILogger<FluentAsynValidationFilter<T>> logger;
    public FluentAsynValidationFilter(ILogger<FluentAsynValidationFilter<T>> logger)
    {
        this.logger=logger;
    }
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        logger.LogValidationFilterStarted(typeof(T).Name, context.HttpContext.Request.Body);
        try
        {
            var argumentOrNull = context.Arguments.FirstOrDefault(a => a is T);
            IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if(argumentOrNull is T argumentToValidate is false)
            {
                logger.LogValidationFilterArgumentOrNull(typeof(T).Name);
                return Results.BadRequest("Request is missing required object.");
            }
            if(validator is not null)
            {
                logger.LogValidationFilterValidationStarted(typeof(T).Name);
                var validationResult = await validator.ValidateAsync(argumentToValidate);
                if(validationResult.IsValid is false)
                {
                    logger.LogValidationFilterValidationResulProblem(
                        typeof(T).Name,
                        validationResult.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage),
                        (int)HttpStatusCode.BadRequest
                    );
                    return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.UnprocessableEntity);
                }
            }
            logger.LogValidationFilterCompleted(typeof(T).Name);
            return await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogValidationFilterFailedException(typeof(T).Name, ex);
            return Results.Problem(ex.Message, statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}