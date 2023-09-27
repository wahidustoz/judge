namespace Ilmhub.Judge.Api.Validators;
using FluentValidation;
using Ilmhub.Judge.Api.Dtos;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Models;

public class JudgeRequestValidator : AbstractValidator<JudgeRequestDto>
{
    public JudgeRequestValidator(ILanguageService languageService, IJudger judger)
    {
        RuleFor(dto => dto.Source).NotEmpty().MaximumLength(5000)
            .WithMessage("{PropertyValue} MaximumLength is 5000");

        RuleFor(dto => dto.LanguageId).MustAsync(async (request, ctx, cancellation) =>
            {
                var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(request.LanguageId, cancellation);
                return languageConfiguration is not null;
            })
            .WithMessage("Language with id {PropertyValue} not configured.");

        RuleFor(dto => dto.MaxCpu).NotEmpty().NotNull().GreaterThan(0).GreaterThan(-1)
            .WithMessage("MaxCpu is not configured");

        RuleFor(dto => dto.MaxMemory).NotEmpty().NotNull().GreaterThan(0).GreaterThan(-1)
            .WithMessage("MaxMemory is not configured");

        RuleFor(x => x.TestCaseId)
                .NotNull()
                .Must((request, ctx, cancellation) => judger.TestCaseExists(request.TestCaseId))
                .When(x => x.TestCaseId == Guid.Empty)
                .WithMessage("{PropertyName} should have valid value when Testcases are empty.")
                .Null()
                .When(x => x.TestCases?.Any() is true)
                .WithMessage("{PropertyName} must be null when Testcases exist.");

        RuleFor(dto => dto.TestCases)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(x => x.TestCases.Count()).InclusiveBetween(1, 100);
                    RuleFor(x => x.TestCases)
                        .Must(HaveUniqueId)
                        .WithMessage("All {PropertyName} must have unique IDs.");
                })
                .When(x => x.TestCaseId == Guid.Empty)
                .Empty()
                .When(x => x.TestCaseId != Guid.Empty)
                .WithMessage("{PropertyName} must be null when TestcaseId exists.");

        RuleForEach(x => x.TestCases).ChildRules(testcase =>
        {
            testcase.RuleFor(t => t.Id).NotEmpty();
            testcase.RuleFor(t => t.Output).NotEmpty().MaximumLength(5000);
            testcase.RuleFor(t => t.Input).MaximumLength(5000);
        });
    }
    private bool HaveUniqueId(IEnumerable<TestCase> testcases) =>
    testcases.Select(x => x.Id).Distinct().Count() == testcases.Count();
}
