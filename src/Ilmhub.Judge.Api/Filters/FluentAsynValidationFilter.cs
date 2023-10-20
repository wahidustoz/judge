using System.Net;
using FluentValidation;

namespace Ilmhub.Judge.Api.Filters;

public class FluentAsynValidationFilter<T> : IEndpointFilter where T : class {
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var argumentOrNull = context.Arguments.FirstOrDefault(a => a is T);
        IValidator<T> validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (argumentOrNull is T argumentToValidate is false)
            return Results.BadRequest("Request is missing required object.");

        if (validator is not null) {
            var validationResult = await validator.ValidateAsync(argumentToValidate);
            if (validationResult.IsValid is false)
                return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.UnprocessableEntity);
        }
        return await next.Invoke(context);
    }
}