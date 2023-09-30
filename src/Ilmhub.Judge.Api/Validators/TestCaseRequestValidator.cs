using FluentValidation;
using Ilmhub.Judge.Api.Dtos;

namespace Ilmhub.Judge.Api.Validators;

public class TestCaseRequestValidator : AbstractValidator<TestCaseDto>
{
    public TestCaseRequestValidator()
    {
        RuleFor(t => t.Id).NotEmpty();
        RuleFor(t => t.Input).MaximumLength(5000);
        RuleFor(t => t.Output).NotEmpty().MaximumLength(5000);
    }
}