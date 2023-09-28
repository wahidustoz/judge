using System.Net;
using FluentValidation;
using Ilmhub.Judge.Api.Dtos;

namespace Ilmhub.Judge.Api.Filters;

public class FluentAsynValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T argToValidate = context.GetArgument<T>(0);
        IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(argToValidate!);
            if (validationResult.IsValid is false)
                return Results.ValidationProblem(
                    errors: validationResult.ToDictionary(),
                    statusCode: (int)HttpStatusCode.UnprocessableEntity);
        }

        // Otherwise invoke the next filter in the pipeline
        return await next.Invoke(context);
    }
}
