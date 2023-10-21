using System.IO.Compression;
using FluentValidation;

namespace Ilmhub.Judge.Api.Validators;
public class TestCaseFormFileValidator : AbstractValidator<IFormFile>
{
    private readonly int maxBytes = 50 * 1024 * 1024;
    private readonly string zip_extension = ".zip";
    public TestCaseFormFileValidator(ILogger<TestCaseFormFileValidator> logger)
    {
        RuleFor(f => f.Length).ExclusiveBetween(1, maxBytes);
        RuleFor(f => f.FileName)
                .NotEmpty()
                .Must(HaveSupportedFileType)
                .WithMessage($"File extension must be {zip_extension}");

        RuleFor(x => x)
            .Custom((file, context) =>
            {
                try
                {
                    var fileReadAsIFrom = file as IFormFile;
                    using var fileStream = fileReadAsIFrom.OpenReadStream();
                    using var zip = new ZipArchive(fileStream);

                    if (zip.Entries.Count() > 200)
                    {
                        context.AddFailure("Testcases", "There are more than 200 input files.");
                        return;
                    }
                    if (zip.Entries.Count() == 0)
                    {
                        context.AddFailure("Testcases", "There are no input files.");
                        return;
                    }

                    var inputFiles = zip.Entries?.Where(x => x.Name.EndsWith(".in"));
                    var outputFiles = zip.Entries?.Where(x => x.Name.EndsWith(".out"));
                    if (inputFiles.All(i => outputFiles.Any(o => HaveMatchingNames(i, o))) is false)
                    {
                        context.AddFailure("Testcases", "There is no matching output files for input files.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    context.AddFailure("Testcases", "Invalid zip file.");
                    logger.LogWarning(ex, "Invalid zip file.");
                    return;
                }
            });
    }

    private bool HaveMatchingNames(ZipArchiveEntry x, ZipArchiveEntry y)
        => string.Equals(Path.GetFileNameWithoutExtension(x.Name), Path.GetFileNameWithoutExtension(y.Name), StringComparison.OrdinalIgnoreCase);
    private bool HaveSupportedFileType(string fileName)
       => string.Equals(Path.GetExtension(fileName), zip_extension, StringComparison.OrdinalIgnoreCase);
}