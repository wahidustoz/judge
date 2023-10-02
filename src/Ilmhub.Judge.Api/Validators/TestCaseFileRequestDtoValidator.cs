using FluentValidation;

namespace Ilmhub.Judge.Api.Validators;
public class TestCaseFileRequestDtoValidator : AbstractValidator<IFormFile>
{
    private int maxBytes = 50 * 1024 * 1024;
    private string ZIP_EXTENSION = ".zip";
    public TestCaseFileRequestDtoValidator()
    {
        RuleFor(f => f.Length).ExclusiveBetween(1, maxBytes);
        RuleFor(f => f.FileName)
                .NotEmpty()
                .Must(HaveSupportedFileType)
                .WithMessage($"File extension must be {ZIP_EXTENSION}");
    }
    private bool HaveSupportedFileType(string fileName)
       => string.Equals(Path.GetExtension(fileName), ZIP_EXTENSION, StringComparison.OrdinalIgnoreCase);
}