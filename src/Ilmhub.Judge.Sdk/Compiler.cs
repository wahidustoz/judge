using System.Text.Json;
using System.Text.Json.Serialization;
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
    private const string READ_WRITE_EXEC_MODE = "700";
    private const string LOG_FILENAME = "log";
    private const string OUTPUT_FILENAME = "output";
    private const string ERROR_FILENAME = "error";
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
        string externalExecutablePath = default,
        CancellationToken cancellationToken = default)
    {
        string tempFolder = string.Empty;
        try
        {
            var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId, cancellationToken);
            if(languageConfiguration is null)
                throw new LanguageNotConfiguredException(languageId);

            logger.LogTrace("Starting compilation session for language {languageId}", languageId);

            tempFolder = await CreateTemporaryFolderAsync(cancellationToken);
            var sourceFilename = await WriteSourceFileAsync(source, tempFolder, languageConfiguration, cancellationToken);
            (string logPath, string outputPath, string errorPath) = 
                await CreateInputOutputFilesAsync(tempFolder, cancellationToken);
            var executableFilename = await WriteExecutableFileAsync(externalExecutablePath, tempFolder, languageConfiguration, cancellationToken);
            var executablePath = Path.GetDirectoryName(executableFilename);

            var arguments = languageConfiguration.Compile.Arguments
                .Select(argument => 
                {
                    if(argument.Contains("{src_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{src_path}", sourceFilename);
                    else if(argument.Contains("{exe_dir}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_dir}", executablePath);
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
                logger.LogTrace("Finished compilation session for language {languageId}", languageId);

                (string log, string output, string error) = 
                    await TryReadCompilerOutputFiles(logPath, outputPath, errorPath, cancellationToken);

                return new CompilationResult(processResult)
                {
                    Log = log,
                    Output = output,
                    Error = error
                };
        }
        catch(Exception ex) when (ex is not LanguageNotConfiguredException)
        {
            logger.LogError(ex, "Failed to execute compiler for language {languageId}.", languageId);
            throw new CompilationProcessFailedException($"Failed to execute compiler for language {languageId}.", ex);
        }
        finally
        {
            logger.LogTrace("Deleting temporary folder: {tempFolder}", tempFolder);
            await cli.RemoveFolderAsync(tempFolder, cancellationToken);
        }
    }

    private async ValueTask<(string log, string output, string error)> TryReadCompilerOutputFiles(
        string logPath, string outputPath, string errorPath, CancellationToken cancellationToken)
    {
        string log = null; 
        string output = null; 
        string error = null;

        if(Path.Exists(logPath))
            log = await File.ReadAllTextAsync(logPath, cancellationToken);
        if(Path.Exists(outputPath))
            output = await File.ReadAllTextAsync(outputPath, cancellationToken);
        if(Path.Exists(outputPath))
            error = await File.ReadAllTextAsync(errorPath, cancellationToken);

        return (log, output, error);
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
            await cli.ChangePathModeAsync(READ_WRITE_MODE, path, cancellationToken: cancellationToken); 
        }

        logger.LogTrace(
            "Created Compiler output files. \nLog Path {logPath}, \nOutput Path {outputPath}, \nError Path {errorPath}", 
            logPath, 
            outputPath, 
            errorPath);

        return (logPath, outputPath, errorPath);
    }

    private async ValueTask<string> WriteExecutableFileAsync(
        string externalExecutablePath, 
        string tempFolder, 
        ILanguageConfiguration languageConfiguration, 
        CancellationToken cancellationToken)
    {
        var executablePath = IsValidPath(externalExecutablePath) ? externalExecutablePath : tempFolder;
        var executableFilename = Path.Combine(executablePath, languageConfiguration.Compile.ExecutableName);
        IOUtilities.CreateEmptyFile(executableFilename);
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, executablePath, true, cancellationToken);
        await cli.ChangePathModeAsync(READ_WRITE_EXEC_MODE, executablePath, true, cancellationToken);
        logger.LogInformation("Created executable file: {executableFilename}", executableFilename);

        return executableFilename;
    }

    private static bool IsValidPath(string path) 
        => string.IsNullOrWhiteSpace(path) is false 
        && Directory.Exists(path) is true;

    private async ValueTask<string> WriteSourceFileAsync(
        string source, 
        string tempFolder, 
        ILanguageConfiguration languageConfiguration, 
        CancellationToken cancellationToken)
    {
        var sourceName = languageConfiguration.Compile.SourceName;
        if(string.IsNullOrWhiteSpace(Path.GetDirectoryName(sourceName)) is false)
            throw new InvalidLanguageConfigurationException(languageConfiguration);

        var sourceFilename = Path.Combine(tempFolder, sourceName);
        await File.WriteAllTextAsync(sourceFilename, source, cancellationToken);
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, sourceFilename, cancellationToken: cancellationToken);
        await cli.ChangePathModeAsync(READ_MODE, sourceFilename, cancellationToken: cancellationToken);
        logger.LogTrace("Created source file: {sourceFilename}", sourceFilename);

        return sourceFilename;
    }

    private async ValueTask<string> CreateTemporaryFolderAsync(CancellationToken cancellationToken)
    {
        var folder = IOUtilities.CreateTemporaryDirectory();
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, folder, cancellationToken: cancellationToken);
        logger.LogTrace("Created temporary folder: {tempFolder}", folder);

        return folder;
    }
}