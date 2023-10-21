using FluentValidation;
using Ilmhub.Judge.Api.Dtos;

namespace Ilmhub.Judge.Api.Validators;

public class TestCaseRequestValidator : AbstractValidator<IEnumerable<TestCaseDto>>
{
    public TestCaseRequestValidator()
    {
        RuleFor(x => x.Count()).InclusiveBetween(1, 100);

        RuleFor(x => x)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .Must(HaveUniqueId)
                    .WithMessage("All {PropertyName} must have unique IDs.");
            });

        RuleForEach(x => x).ChildRules(testcase =>
        {
            testcase.RuleFor(t => t.Id).NotEmpty();
            testcase.RuleFor(t => t.Input).MaximumLength(5000);
            testcase.RuleFor(t => t.Output).NotEmpty().MaximumLength(5000);
        });
    }

    private bool HaveUniqueId(IEnumerable<TestCaseDto> testcases)
        => testcases.Select(x => x.Id).Distinct().Count() == testcases.Count();
}