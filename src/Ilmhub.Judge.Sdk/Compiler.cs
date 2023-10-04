using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Wrapper;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Judge.Wrapper.Models;
using Ilmhub.Utilities;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public class Compiler : ICompiler
{
    private const string LOG_FILENAME = "compiler.log";
    private const string OUTPUT_FILENAME = "compiler.output";
    private const string ERROR_FILENAME = "compiler.error";
    private readonly ILogger<Compiler> logger;
    private readonly ILinuxCommandLine cli;
    private readonly IJudgeWrapper judgeWrapper;
    private readonly ILanguageService languageService;
    private readonly IJudgeUsersOption judgeUsers;

    private bool shouldCleanUpEnvironmentFolder = false;

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

    public bool CanHandle(int languageId) => languageService.IsSupportedDotnetVersion(languageId) is false;

    public async ValueTask<ICompilationResult> CompileAsync(
        string source, 
        int languageId,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId, cancellationToken) 
                ?? throw new LanguageNotConfiguredException(languageId);

            logger.LogInformation("Starting compilation session for language {languageId}", languageId);

            if(IOUtilities.IsValidPath(environmentFolder) is false)
                environmentFolder = await CreateTemporaryFolderAsync(cancellationToken);

            var sourceFilename = await WriteSourceFileAsync(source, environmentFolder, languageConfiguration, cancellationToken);
            var executableFilename = await CreateExecutableFileAsync(environmentFolder, languageConfiguration.Compile.ExecutableName, cancellationToken);
            (string logPath, string outputPath, string errorPath) = await CreateInputOutputFilesAsync(environmentFolder, cancellationToken);

            var arguments = languageConfiguration.Compile.Arguments?
                .Select(argument => 
                {
                    if(argument.Contains("{src_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{src_path}", sourceFilename);
                    else if(argument.Contains("{exe_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_path}", executableFilename);

                    return argument;
                });

            var processResult = await judgeWrapper.ExecuteJudgerAsync(
                request: new ExecutionRequest
                {
                    ExecutablePath = languageConfiguration.Compile.Command,
                    OutputPath = outputPath,
                    LogPath = logPath,
                    ErrorPath = errorPath,
                    Arguments = arguments,
                    Environments = languageConfiguration.Compile.EnvironmentVariables,
                    CpuTime = languageConfiguration.Compile.MaxCpuTime,
                    RealTime = languageConfiguration.Compile.MaxRealTime,
                    Memory = languageConfiguration.Compile.MaxMemory,
                    Uid = judgeUsers.Compiler.UserId,
                    Gid = judgeUsers.Compiler.GroupId
                },
                cancellationToken: cancellationToken);
                logger.LogInformation("Finished compilation session for language {languageId}", languageId);

                return new CompilationResult(processResult)
                {
                    Log = await IOUtilities.GetAllTextOrDefaultAsync(logPath, cancellationToken),
                    Output = await IOUtilities.GetAllTextOrDefaultAsync(outputPath, cancellationToken),
                    Error = await IOUtilities.GetAllTextOrDefaultAsync(errorPath, cancellationToken),
                    ExecutableFilePath = executableFilename
                };
        }
        catch(Exception ex) when (ex is not LanguageNotConfiguredException)
        {
            logger.LogError(ex, "Failed to execute compiler for language {languageId}.", languageId);
            throw new CompilationProcessFailedException($"Failed to execute compiler for language {languageId}.", ex);
        }
        finally
        {
            if(shouldCleanUpEnvironmentFolder)
            {
                logger.LogInformation("Deleting temporary folder: {tempFolder}", environmentFolder);
                await cli.RemoveFolderAsync(environmentFolder, cancellationToken);
            }
        }
    }

    private async ValueTask<string> CreateExecutableFileAsync(
        string environmentFolder, 
        string executableFilename, 
        CancellationToken cancellationToken = default)
    {
        var executableFilePath = Path.Combine(environmentFolder, executableFilename);
        IOUtilities.CreateEmptyFile(executableFilePath);
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, executableFilePath, cancellationToken: cancellationToken);
        logger.LogInformation("Created executable file: {executableFilename}", executableFilePath);

        var executableFolder = Path.GetDirectoryName(executableFilePath);
        if(string.IsNullOrWhiteSpace(Path.GetDirectoryName(executableFilename)) is false)
            await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, executableFolder, cancellationToken: cancellationToken);

        return executableFilePath;
    }

    private async ValueTask<(string logPath, string outputPath, string errorPath)> CreateInputOutputFilesAsync(
        string tempFolder, 
        CancellationToken cancellationToken)
    {
        var logPath = Path.Combine(tempFolder, LOG_FILENAME);
        var outputPath = Path.Combine(tempFolder, OUTPUT_FILENAME);
        var errorPath = Path.Combine(tempFolder, ERROR_FILENAME);

        var paths = new string[] { logPath, outputPath, errorPath };
        foreach(var path in paths)
        {
            IOUtilities.CreateEmptyFile(path);
            await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, path, cancellationToken: cancellationToken);
            await cli.ChangePathModeAsync(LinuxCommandLine.WRITE_MODE, path, cancellationToken: cancellationToken); 
        }

        logger.LogInformation(
            "Created Compiler output files. \nLog Path {logPath}, \nOutput Path {outputPath}, \nError Path {errorPath}", 
            logPath, 
            outputPath, 
            errorPath);

        return (logPath, outputPath, errorPath);
    }

    private async ValueTask<string> WriteSourceFileAsync(
        string source, 
        string environtmentFolder, 
        ILanguageConfiguration languageConfiguration, 
        CancellationToken cancellationToken)
    {
        var sourceName = languageConfiguration.Compile.SourceName;
        if(string.IsNullOrWhiteSpace(Path.GetDirectoryName(sourceName)) is false)
            throw new InvalidLanguageConfigurationException(languageConfiguration);

        var sourceFilename = Path.Combine(environtmentFolder, sourceName);
        await File.WriteAllTextAsync(sourceFilename, source, cancellationToken);
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, sourceFilename, cancellationToken: cancellationToken);
        await cli.ChangePathModeAsync(LinuxCommandLine.READ_MODE, sourceFilename, cancellationToken: cancellationToken);
        logger.LogInformation("Created source file: {sourceFilename}", sourceFilename);

        return sourceFilename;
    }

    private async ValueTask<string> CreateTemporaryFolderAsync(CancellationToken cancellationToken)
    {
        var folder = IOUtilities.CreateTemporaryDirectory();
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, folder, cancellationToken: cancellationToken);
        logger.LogInformation("Created temporary folder: {tempFolder}", folder);

        shouldCleanUpEnvironmentFolder = true;

        return folder;
    }
}