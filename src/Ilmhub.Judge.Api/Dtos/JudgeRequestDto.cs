using FluentValidation;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Api.Dtos;

public class JudgeRequestDto
{
    public int LanguageId { get; set; }
    public string Source { get; set; }
    public long MaxCpu { get; set; } = -1;
    public long MaxMemory { get; set; } = -1;
    public Guid TestCaseId { get; set; }
    public IEnumerable<TestCase> TestCases { get; set; }

    public class JudgeRequestValidator : AbstractValidator<JudgeRequestDto>
    {
        public JudgeRequestValidator(ILanguageService languageService, IJudger judger)
        {
            RuleFor(dto => dto.Source).NotEmpty();

            RuleFor(dto => dto.LanguageId).MustAsync(async (request, ctx, cancellation) =>
            {
                var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(request.LanguageId, cancellation);
                return languageConfiguration is not null;
            }).WithMessage("LanguageId not configured");

            RuleFor(dto => dto.MaxCpu).NotEmpty().GreaterThan(0).GreaterThan(-1)
                .WithMessage("MaxCpu is not configured");

            RuleFor(dto => dto.MaxMemory).NotEmpty().GreaterThan(0).GreaterThan(-1)
                .WithMessage("MaxMemory is not configured");


            RuleFor(dto => dto.TestCaseId)
                .NotEmpty()
                .Must((request, ctx, cancellation) =>
                {
                    bool testCase = judger.TestCaseExists(request.TestCaseId);
                    return testCase;
                });

            RuleFor(dto => dto.TestCases)
            .Must(dto => dto.Count() < 100)
            .Must(dto => dto.Select(dto => dto.Id).Distinct().Count() == dto.Count())
            .WithMessage("TestCaseId is not configured");

            RuleForEach(dto => dto.TestCases)
                .NotEmpty()
                .SetValidator(new TestCaseValidator())
                .WithMessage("TestCase is not configured");
        }

        private class TestCaseValidator : AbstractValidator<TestCase>
        {
            public TestCaseValidator()
            {
                RuleFor(dto => dto.Id).NotEmpty();
                RuleFor(dto => dto.Input).MaximumLength(5000);
                RuleFor(dto => dto.Output).MaximumLength(5000).NotEmpty();
            }
        }
    }

}
