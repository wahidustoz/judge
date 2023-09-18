using System.Diagnostics;
using Ilmhub.Judge.Api.Models;
using Ilmhub.Judge.Api.Options;
using Ilmhub.Judge.Api.Services;
using Microsoft.Extensions.Options;

namespace Ilmhub.Judge.Api.Services;

public class FileService
{
    private JudgeEnvironmentOptions options;
    private readonly ILogger<FileService> logger;
    private readonly LanguageService languageService;

    public FileService(ILogger<FileService> logger, IOptions<JudgeEnvironmentOptions> options, LanguageService languageService)
    {
        this.options = options.Value;
        this.logger = logger;
        this.languageService = languageService;
    }

    public async ValueTask<SubmissionEnvironment> InitializeSubmissionEnvironmentAsync(
        int languageId, 
        string source, 
        IEnumerable<Testcase> testcases, 
        CancellationToken cancellationToken = default)
    {
        var result = new SubmissionEnvironment();
        var tempSubmissionFolder = Guid.NewGuid().ToString();

        if(Directory.Exists(options.RootPath) is false)
            throw new Exception($"Root path {options.RootPath} does not exist");
        
        var submissionPath = Directory.CreateDirectory(Path.Combine(options.RootPath, tempSubmissionFolder));
        var sourcePath = Directory.CreateDirectory(Path.Combine(options.RootPath, $"{tempSubmissionFolder}/source"));
        var testcasesPath = Directory.CreateDirectory(Path.Combine(options.RootPath, $"{tempSubmissionFolder}/testcases"));
        var executablePath = Directory.CreateDirectory(Path.Combine(options.RootPath, $"{tempSubmissionFolder}/executable"));

        try
        {
            Process.Start("chown", $"{options.Compiler.Username} {submissionPath.FullName}");
            Process.Start("chown", $"{options.Compiler.Username} {sourcePath.FullName}");
            Process.Start("chown", $"{options.Runner.Username} {executablePath.FullName}");
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex, "Failed to change ownership");
        }

        var languageConfiration = await languageService.GetLanguageConfigurationAsync(languageId, cancellationToken);
        if(languageConfiration.Compile is not null)
        {
            File.WriteAllText(Path.Combine(sourcePath.FullName, languageConfiration.Compile.SourceName), source);
            Process.Start("chown", $"{options.Compiler.Username} {sourcePath.FullName}/{languageConfiration.Compile.SourceName}");
            Process.Start("chmod", $"400 {sourcePath.FullName}/{languageConfiration.Compile.SourceName}");
        }
        else
        {
            File.WriteAllText(Path.Combine(executablePath.FullName, languageConfiration.Run.ExecutableName), source);
            Process.Start("chown", $"{options.Runner.Username} {sourcePath.FullName}/{languageConfiration.Compile.SourceName}");
            Process.Start("chmod", $"700 {sourcePath.FullName}/{languageConfiration.Compile.SourceName}");
        }

        int fileId = 1;
        foreach(var testcase in testcases)
        {
            File.WriteAllText(Path.Combine(testcasesPath.FullName, $"{fileId}.in"), testcase?.Input);
            Process.Start("chown", $"{options.Runner.Username} {testcasesPath.FullName}/{fileId}.in");
            Process.Start("chmod", $"400 {testcasesPath.FullName}/{fileId}.in");

            File.WriteAllText(Path.Combine(testcasesPath.FullName, $"{fileId}.out"), testcase?.Output);
            Process.Start("chown", $"{options.Runner.Username} {testcasesPath.FullName}/{fileId}.out");
            Process.Start("chmod", $"400 {testcasesPath.FullName}/{fileId}.out");

            result.Testcases.Add(Path.Combine(testcasesPath.FullName, $"{fileId}.in"), Path.Combine(testcasesPath.FullName, $"{fileId}.out"));
        }

        result.SourcePath = Path.Combine(sourcePath.FullName, languageConfiration.Compile.SourceName);
        result.ExecutablePath = executablePath.FullName;
        result.TestcasesPath = testcasesPath.FullName;

        return result;
    }
}