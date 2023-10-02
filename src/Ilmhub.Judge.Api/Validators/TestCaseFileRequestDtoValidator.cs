using System.Net.Mime;
using FluentValidation;
using Ilmhub.Judge.Api.Dtos;

namespace Ilmhub.Judge.Api.Validators;
public class TestCaseFileRequestDtoValidator : AbstractValidator<IFormFile>
{
    int maxBytes = 50 * 1024 * 1024;
    string supportedFileType = null;
    public TestCaseFileRequestDtoValidator()
    {
        RuleFor(f => f.FileName).NotEmpty().Equals("testcases");
        RuleFor(f => f.Length).ExclusiveBetween(1, maxBytes)
            .WithMessage($"File length should be greater than 0 and less than {maxBytes / 1024 / 1024} MB");
        RuleFor(f => f.FileName)
                .NotEmpty()
                .Must(fileName => HaveSupportedFileType(fileName))
                .WithMessage($"File extension should be zip");
    }
    private bool HaveSupportedFileType(string fileName)
    {
        supportedFileType = Path.GetExtension(fileName).Trim('.'); // Get the file extension
        return supportedFileType.Equals("zip", StringComparison.InvariantCultureIgnoreCase);
    }
}