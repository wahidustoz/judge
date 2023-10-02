using System.Net;
using FluentValidation;

namespace Ilmhub.Judge.Api.Filters;

public class FluentAsynValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argumentOrNull = context.Arguments.FirstOrDefault(a => a is T);
        IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null && argumentOrNull is T argumentToValidate)
        {
            var validationResult = await validator.ValidateAsync(argumentToValidate);
            if (validationResult.IsValid is false)
                return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.UnprocessableEntity);
        }
        else
        {
            return Results.BadRequest("Input object is missing or invalid.");
        }

        return await next.Invoke(context);
    }
}