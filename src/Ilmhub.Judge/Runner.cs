using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Exceptions;
using Ilmhub.Judge.Models;
using Ilmhub.Judge.Wrapper;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Judge.Wrapper.Models;
using Ilmhub.Utilities;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge;

public class Runner : IRunner
{
    private const string INPUT_FILENAME = "input";
    private const string OUTPUT_FILENAME = "runner.output";
    private const string ERROR_FILENAME = "runner.error";
    private const string LOG_FILENAME = "runner.log";
    private readonly ILogger<Runner> logger;
    private readonly ILinuxCommandLine cli;
    private readonly IJudgeWrapper judgeWrapper;
    private readonly ILanguageService languageService;
    private readonly IJudgeUsersOption judgeUsers;
    private bool shouldCleanUpEnvironmentFolder = false;

    public Runner(
        ILogger<Runner> logger,
        ILinuxCommandLine cli,
        IJudgeWrapper judgeWrapper,
        ILanguageService languageService,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.cli = cli;
        this.judgeWrapper = judgeWrapper;
        this.languageService = languageService;
        judgeUsers = options.SystemUsers;
    }

    public async ValueTask<IRunnerResult> RunAsync(
        int languageId,
        string executableFilename,
        long maxCpu,
        long maxMemory,
        string input = default,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        if (IOUtilities.IsValidPath(environmentFolder) is false)
            environmentFolder = await CreateTemporaryFolderAsync(cancellationToken);

        string inputFilePath = string.Empty;
        if (string.IsNullOrWhiteSpace(input) is false)
            inputFilePath = await WriteInputFileAsync(input, environmentFolder, cancellationToken);

        return await RunInternalAsync(
            languageId,
            inputFilePath,
            executableFilename,
            maxCpu,
            maxMemory,
            environmentFolder,
            cancellationToken);
    }

    public async ValueTask<IRunnerResult> RunAsync(
        int languageId,
        string inputFilePath,
        string executableFilename,
        long maxCpu,
        long maxMemory,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        if (IOUtilities.IsValidPath(environmentFolder) is false)
            environmentFolder = await CreateTemporaryFolderAsync(cancellationToken);

        return await RunInternalAsync(
            languageId,
            inputFilePath,
            executableFilename,
            maxCpu,
            maxMemory,
            environmentFolder,
            cancellationToken);
    }

    private async ValueTask<IRunnerResult> RunInternalAsync(
        int languageId,
        string inputFilePath,
        string executableFilename,
        long maxCpu,
        long maxMemory,
        string environmentFolder,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId, cancellationToken)
                ?? throw new LanguageNotConfiguredException(languageId);

            logger.LogInformation("Starting runner session for language {languageId}", languageId);

            (string logPath, string outputPath, string errorPath) = await CreateInputOutputFilesAsync(environmentFolder, cancellationToken);
            var executableFolder = Path.GetDirectoryName(executableFilename);
            var arguments = languageConfiguration.Run.Arguments?
                .Select(argument =>
                {
                    if (argument.Contains("{exe_dir}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_dir}", executableFolder);
                    else if (argument.Contains("{exe_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_path}", executableFilename);

                    return argument;
                });

            await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, executableFolder, cancellationToken: cancellationToken);
            await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, executableFilename, cancellationToken: cancellationToken);
            await cli.ChangePathModeAsync(LinuxCommandLine.EXECUTE_MODE, executableFilename, cancellationToken: cancellationToken);

            var executable = languageConfiguration.Run.Command.Contains("{exe_path}")
            ? languageConfiguration.Run.Command.Replace("{exe_path}", executableFilename)
            : languageConfiguration.Run.Command;

            var processResult = await judgeWrapper.ExecuteJudgerAsync(
                request: new ExecutionRequest
                {
                    ExecutablePath = executable,
                    InputPath = inputFilePath,
                    OutputPath = outputPath,
                    LogPath = logPath,
                    ErrorPath = errorPath,
                    Arguments = arguments,
                    Environments = languageConfiguration.Run.EnvironmentVariables,
                    CpuTime = maxCpu,
                    Memory = maxMemory,
                    Uid = judgeUsers.Runner.UserId,
                    Gid = judgeUsers.Runner.GroupId
                },
                cancellationToken: cancellationToken);
            logger.LogInformation("Finished runner session for language {languageId}", languageId);

            var log = await IOUtilities.GetAllTextOrDefaultAsync(logPath, cancellationToken);
            var output = await IOUtilities.GetAllTextOrDefaultAsync(outputPath, cancellationToken);
            var error = await IOUtilities.GetAllTextOrDefaultAsync(errorPath, cancellationToken);

            return new RunnerResult(
                execution: processResult,
                output: output,
                error: error,
                log: log);
        }
        catch (Exception ex) when (ex is not LanguageNotConfiguredException)
        {
            logger.LogError(ex, "Failed to execute compiler for language {languageId}.", languageId);
            throw new CompilationProcessFailedException($"Failed to execute compiler for language {languageId}.", ex);
        }
        finally
        {
            if (shouldCleanUpEnvironmentFolder)
            {
                logger.LogInformation("Deleting temporary folder: {tempFolder}", environmentFolder);
                await cli.RemoveFolderAsync(environmentFolder, cancellationToken);
            }
        }
    }

    private async ValueTask<(string logPath, string outputPath, string errorPath)> CreateInputOutputFilesAsync(
        string tempFolder,
        CancellationToken cancellationToken)
    {
        var logPath = Path.Combine(tempFolder, LOG_FILENAME);
        var outputPath = Path.Combine(tempFolder, OUTPUT_FILENAME);
        var errorPath = Path.Combine(tempFolder, ERROR_FILENAME);

        var paths = new string[] { logPath, outputPath, errorPath };
        foreach (var path in paths)
        {
            IOUtilities.CreateEmptyFile(path);
            await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, path, cancellationToken: cancellationToken);
            await cli.ChangePathModeAsync(LinuxCommandLine.WRITE_MODE, path, cancellationToken: cancellationToken);
        }

        logger.LogInformation(
            "Created Compiler output files. \nLog Path {logPath}, \nOutput Path {outputPath}, \nError Path {errorPath}",
            logPath,
            outputPath,
            errorPath);

        return (logPath, outputPath, errorPath);
    }

    private async ValueTask<string> WriteInputFileAsync(
        string input,
        string environmentFolder,
        CancellationToken cancellationToken)
    {

        var inputFilename = Path.Combine(environmentFolder, INPUT_FILENAME);
        await File.WriteAllTextAsync(inputFilename, input, cancellationToken);
        await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, inputFilename, cancellationToken: cancellationToken);
        await cli.ChangePathModeAsync(LinuxCommandLine.READ_MODE, inputFilename, cancellationToken: cancellationToken);
        logger.LogInformation("Created input file: {sourceFilename}", inputFilename);

        return inputFilename;
    }

    private async ValueTask<string> CreateTemporaryFolderAsync(CancellationToken cancellationToken)
    {
        var folder = IOUtilities.CreateTemporaryDirectory();
        await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, folder, cancellationToken: cancellationToken);
        logger.LogInformation("Created temporary folder: {tempFolder}", folder);

        shouldCleanUpEnvironmentFolder = true;

        return folder;
    }
}