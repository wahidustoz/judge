using FluentValidation;
using Ilmhub.Judge.Api.Dtos;

namespace Ilmhub.Judge.Api.Validators;
public class TestCaseFileRequestDtoValidator : AbstractValidator<TestCaseFileDto>
{
    int maxBytes = 50 * 1024 * 1024;
    string supportedFileType = null;
    public TestCaseFileRequestDtoValidator()
    {
        supportedFileType = Path.GetExtension("testcases");
        RuleFor(f => f.formFile.FileName).NotEmpty().Equals("testcases");
        RuleFor(f => f.formFile.Length).ExclusiveBetween(1, maxBytes)
            .WithMessage($"File length should be greater than 0 and less than {maxBytes / 1024 / 1024} MB");
        RuleFor(f => f.formFile.FileName).Must(HaveSupportedFileType)
            .WithMessage($"File extension should be {supportedFileType}");
    }
    private bool HaveSupportedFileType(string fileName)
    {
        return supportedFileType.Equals(fileName, StringComparison.InvariantCultureIgnoreCase);
    }
}