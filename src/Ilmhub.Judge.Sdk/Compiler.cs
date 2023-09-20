using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Judge.Wrapper.Abstractions.Models;
using Ilmhub.Judge.Wrapper.Models;
using Ilmhub.Utilities;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public class Compiler : ICompiler
{
    private const string READ_MODE = "400";
    private const string READ_WRITE_MODE = "600";

    private readonly ILogger<Compiler> logger;
    private readonly ILinuxCommandLine cli;
    private readonly IJudgeWrapper judgeWrapper;
    private readonly ILanguageService languageService;
    private readonly IJudgeUsersOption judgeUsers;

    public Compiler(
        ILogger<Compiler> logger, 
        ILinuxCommandLine cli,
        IJudgeWrapper judgeWrapper,
        ILanguageService languageService,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.cli = cli;
        this.judgeWrapper = judgeWrapper;
        this.languageService = languageService;
        this.judgeUsers = options.SystemUsers;
    }

    public async ValueTask<ICompilationResult> CompileAsync(
        string source, 
        int languageId,
        string executableFilePath = default,
        CancellationToken cancellationToken = default)
    {
        string tempFolder = string.Empty;
        try
        {
            var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId, cancellationToken);
            if(languageConfiguration is null)
                throw new LanguageNotConfiguredException(languageId);

            logger.LogInformation("Starting compilation session for language {languageId}", languageId);

            // create temp folder and assign compiler as owner
            tempFolder = IOUtilities.CreateTemporaryDirectory();
            await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, tempFolder, cancellationToken);
            logger.LogInformation("Created temporary folder: {tempFolder}", tempFolder);

            // create source file, add compiler as owner and change mode to readonly
            var sourceFilename = Path.Combine(tempFolder, languageConfiguration.Compile.SourceName);
            await File.WriteAllTextAsync(sourceFilename, source, cancellationToken);
            await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, sourceFilename, cancellationToken);
            await cli.ChangePathModeAsync(READ_MODE, sourceFilename, cancellationToken);
            logger.LogInformation("Created source file: {sourceFilename}", sourceFilename);

            // create executable file, add compiler as owner and change mode to read/write
            var executableFilename = string.IsNullOrWhiteSpace(executableFilePath) || Directory.Exists(executableFilePath) is false
                ? Path.Combine(tempFolder, languageConfiguration.Compile.ExecutableName)
                : Path.Combine(executableFilePath, languageConfiguration.Compile.ExecutableName);
            IOUtilities.CreateEmptyFile(executableFilename);
            await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, executableFilename, cancellationToken);
            await cli.ChangePathModeAsync(READ_WRITE_MODE, executableFilename, cancellationToken);
            logger.LogInformation("Created executable file: {executableFilename}", executableFilename);

            var arguments = languageConfiguration.Compile.Arguments
                .Select(argument => 
                {
                    if(argument.Contains("{src_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{src_path}", sourceFilename);
                    else if(argument.Contains("{exe_dir}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_dir}", tempFolder);
                    else if(argument.Contains("{exe_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_path}", executableFilename);

                    return argument;
                });

            // run compiler
            var processResult = await judgeWrapper.ExecuteJudgerAsync(
                request: new ExecutionRequest
                {
                    ExecutablePath = languageConfiguration.Compile.Command,
                    Arguments = arguments,
                    Environments = languageConfiguration.Compile.EnvironmentVariables,
                    CpuTime = languageConfiguration.Compile.MaxCpuTime,
                    RealTime = languageConfiguration.Compile.MaxRealTime,
                    Memory = languageConfiguration.Compile.MaxMemory,
                    Uid = judgeUsers.Compiler.UserId,
                    Gid = judgeUsers.Compiler.GroupId
                },
                cancellationToken: cancellationToken);

                if(processResult.Status is EExecutionResult.Success && processResult.Error is EExecutionError.Success)
                    logger.LogInformation("Compilation successful for language {languageId}", languageId);

                return new CompilationResult(processResult);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to execute compiler for language {languageId}.", languageId);
            throw new CompilationProcessFailedException($"Failed to execute compiler for language {languageId}.", ex);
        }
        finally
        {
            logger.LogInformation("Deleting temporary folder: {tempFolder}", tempFolder);
            await cli.RemoveFolderAsync(tempFolder, cancellationToken);
        }
    }
}