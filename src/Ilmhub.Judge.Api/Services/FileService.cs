using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            // Process.Start("chown", $"{options.Compiler.Username} {submissionPath.FullName}");
            // Process.Start("chown", $"{options.Compiler.Username} {sourcePath.FullName}");
            // Process.Start("chown", $"{options.Runner.Username} {executablePath.FullName}");
            // Process.Start("chown", $"{options.Runner.Username} {testcasesPath.FullName}");
            // Process.Start("chmod", $"666 {testcasesPath.FullName}");
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex, "Failed to change ownership");
        }

        var languageConfiguration = await languageService.GetLanguageConfigurationAsync(languageId, cancellationToken);
        if(languageConfiguration.Compile is not null)
        {
            if(languageConfiguration.Id == 10)
            {
                await GenerateDotnetSourceAsync(source, sourcePath.FullName, languageConfiguration.Compile);
                
            }
            else
            {
                File.WriteAllText(Path.Combine(sourcePath.FullName, languageConfiguration.Compile.SourceName), source);
                Process.Start("chown", $"{options.Compiler.Username} {sourcePath.FullName}/{languageConfiguration.Compile.SourceName}");
                Process.Start("chmod", $"400 {sourcePath.FullName}/{languageConfiguration.Compile.SourceName}");

                var file = File.Create(Path.Combine(executablePath.FullName, languageConfiguration.Compile.ExecutableName));
                file.Close();
                Process.Start("chown", $"{options.Compiler.Username} {Path.Combine(executablePath.FullName, languageConfiguration.Compile.ExecutableName)}");
                Process.Start("chmod", $"600 {Path.Combine(executablePath.FullName, languageConfiguration.Compile.ExecutableName)}");
            }
            
        }
        else
        {
            File.WriteAllText(Path.Combine(executablePath.FullName, languageConfiguration.Run.ExecutableName), source);
            Process.Start("chmod", $"700 {sourcePath.FullName}/{languageConfiguration.Compile.SourceName}");
        }

        int fileId = 1;
        foreach(var testcase in testcases)
        {
            File.WriteAllText(Path.Combine(testcasesPath.FullName, $"{fileId}.in"), testcase?.Input);
            Process.Start("chmod", $"400 {testcasesPath.FullName}/{fileId}.in");

            File.WriteAllText(Path.Combine(testcasesPath.FullName, $"{fileId}.out"), testcase?.Output);
            Process.Start("chmod", $"400 {testcasesPath.FullName}/{fileId}.out");

            result.Testcases.Add(Path.Combine(testcasesPath.FullName, $"{fileId}.in"), Path.Combine(testcasesPath.FullName, $"{fileId}.out"));
            fileId++;
        }

        result.ExecutableFilePath = Path.Combine(executablePath.FullName, languageConfiguration.Compile.ExecutableName);
        result.SourcePath = Path.Combine(sourcePath.FullName, languageConfiguration.Compile.SourceName);
        result.ExecutablePath = executablePath.FullName;
        result.TestcasesPath = testcasesPath.FullName;

        return result; 
    }

    private async ValueTask GenerateDotnetSourceAsync(string source, string sourceFolder, CompileConfiguration compile)
    {
        var dotnetFolder = Path.Combine(options.RootPath, "dotnet");
        if(Directory.Exists(dotnetFolder) is false || Directory.EnumerateFiles(dotnetFolder).Any() is false)
        {
            logger.LogWarning("Dotnet template source folder is empty. Generating new template");
            var dotnetProcess = Process.Start("dotnet", $"new console -o {dotnetFolder} -n app");
            dotnetProcess.WaitForExit();
        }

        await File.WriteAllTextAsync(Path.Combine(dotnetFolder, "Program.cs"), source);
        Process.Start("pwd", "");
        Process.Start("whoami", "");
        Process.Start("ls", "/judger/dotnet");

        Process.Start("cp", $"-r {dotnetFolder}/. {sourceFolder}");
        Process.Start("rm", $"-rf {sourceFolder}/obj");

        Process.Start("chown", $"compiler {sourceFolder}");
    }
}