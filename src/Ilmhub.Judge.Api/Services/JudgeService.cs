using System.Text.Json;
using System.Text.RegularExpressions;
using Ilmhub.Judge.Api.Models;
using Ilmhub.Judge.Api.Options;
using Microsoft.Extensions.Options;

namespace Ilmhub.Judge.Api.Services;


public class JudgeService
{
    private readonly ILogger<JudgeService> logger;
    private readonly JudgeEnvironmentOptions options;
    private readonly FileService fileService;
    private readonly LanguageService languageService;
    private readonly RunnerService runnerService;

    public JudgeService(
        ILogger<JudgeService> logger, 
        FileService fileService, 
        LanguageService languageService, 
        RunnerService runnerService,
        IOptions<JudgeEnvironmentOptions> options)
    {
        this.fileService = fileService;
        this.languageService = languageService;
        this.runnerService = runnerService;
        this.logger = logger;
        this.options = options.Value;
    }

    public async ValueTask<string> JudgeAsync(JudgeRequest request, CancellationToken cancellationToken = default)
    {
        var submissionEnvironment = await fileService.InitializeSubmissionEnvironmentAsync(
            source: request.Source,
            languageId: request.LanguageId,
            testcases: request.Testcases,
            cancellationToken: cancellationToken);
        
        var langConfiguration = await languageService.GetLanguageConfigurationAsync(request.LanguageId, cancellationToken);
        
        if(langConfiguration.Compile is not null)
        {
            var compiledOutputPath = Path.Combine(submissionEnvironment.ExecutablePath, "compiler.out");
            var compileCommand = langConfiguration.Compile.CompileCommand;
            if(string.IsNullOrWhiteSpace(compileCommand) is false)
            {
                compileCommand = compileCommand.Replace("{src_path}", submissionEnvironment.SourcePath);
                compileCommand = compileCommand.Replace("{exe_dir}", submissionEnvironment.ExecutablePath);
                compileCommand = compileCommand.Replace("{exe_path}", compiledOutputPath);
            }

            var cliArgs = SplitCommandLine(compileCommand);

            var compileResult = runnerService.RunAsync(new RunRequest
            {
                CpuTime = langConfiguration.Compile.MaxCpuTime,
                RealTime = langConfiguration.Compile.MaxRealTime,
                Memory = langConfiguration.Compile.MaxMemory,
                Stack = 128 * 1024 * 1024,
                OutputSize = 20 * 1024 * 1024,
                ProcessNumber = -1,
                Uid = options.Compiler.UserId,
                Gid = options.Compiler.GroupId,
                SeccompRuleName = null,
                InputPath = submissionEnvironment.SourcePath,
                OutputPath = compiledOutputPath,
                ExecutablePath = cliArgs.First(),
                Arguments = cliArgs.Skip(1).ToArray()
            });

            return JsonSerializer.Serialize(compileResult);
        }

        return string.Empty;
    }

    static List<string> SplitCommandLine(string commandLine)
    {
        List<string> arguments = new List<string>();
        Regex regex = new Regex(@"[\""]([^\""]+)\""|([^ ]+)");

        foreach (Match match in regex.Matches(commandLine))
        {
            string argument = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            arguments.Add(argument);
        }

        return arguments;
    }
}