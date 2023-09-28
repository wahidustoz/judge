namespace Ilmhub.Judge.Api.Validators;
using FluentValidation;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Models;

public class JudgeRequestValidator : AbstractValidator<JudgeRequestDto>
{
    public JudgeRequestValidator(ILanguageService languageService, IJudger judger)
    {
        RuleFor(x => x.Source)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.LanguageId)
            .MustAsync(async (request, ctx, cancellation) =>
            {
                var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(request.LanguageId, cancellation);
                return languageConfiguration is not null;
            })
            .WithMessage("Language with id {PropertyValue} is not configured.");

        RuleFor(x => x.MaxCpu)
            .GreaterThan(0)
            .When(x => x.MaxCpu is not null);

        RuleFor(x => x.MaxMemory)
            .GreaterThan(0)
            .When(x => x.MaxMemory is not null);

        RuleFor(x => x.TestCaseId)
            .NotNull()
            .Must((request, ctx, cancellation) => judger.TestCaseExists(request.TestCaseId.Value))
            .When(x => x.TestCases?.Any() is false)
            .WithMessage("{PropertyName} should be a valid existing id when Testcases are not submitted.")
            .Null()
            .When(x => x.TestCases?.Any() is true)
            .WithMessage("{PropertyName} must be null when Testcases are submitted.");

        RuleFor(x => x.TestCases)
            .NotEmpty()
            .DependentRules(() =>
            {
                RuleFor(x => x.TestCases.Count()).InclusiveBetween(1, 100);
                RuleFor(x => x.TestCases)
                    .Must(HaveUniqueId)
                    .WithMessage("All {PropertyName} must have unique IDs.");
            })
            .When(x => x.TestCaseId is null)
            .Empty()
            .When(x => x.TestCaseId is not null)
            .WithMessage("{PropertyName} must be null when TestcaseId exists.");

        RuleForEach(x => x.TestCases).ChildRules(testcase =>
        {
            testcase.RuleFor(t => t.Id).NotEmpty();
            testcase.RuleFor(t => t.Output).NotEmpty().MaximumLength(5000);
            testcase.RuleFor(t => t.Input).MaximumLength(5000);
        });
    }
    
    private bool HaveUniqueId(IEnumerable<TestCase> testcases) 
        => testcases.Select(x => x.Id).Distinct().Count() == testcases.Count();
}
