using System.Diagnostics;
using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Exceptions;
using Ilmhub.Judge.Models;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Judge.Wrapper.Models;
using Ilmhub.Utilities;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge;

public class DotnetCompiler : ICompiler
{
    private readonly ILogger<DotnetCompiler> logger;
    private readonly ILinuxCommandLine cli;
    private readonly ILanguageService languageService;
    private readonly IJudgeUsersOption judgeUsers;
    private bool shouldCleanUpEnvironmentFolder = false;

    public DotnetCompiler(
        ILogger<DotnetCompiler> logger,
        ILinuxCommandLine cli,
        ILanguageService languageService,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.cli = cli;
        this.languageService = languageService;
        judgeUsers = options.SystemUsers;
    }

    public bool CanHandle(int languageId) => languageService.IsSupportedDotnetVersion(languageId);

    public async ValueTask<ICompilationResult> CompileAsync(
        string source,
        int languageId,
        string environmentFolder = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var languageConfiguration = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId, cancellationToken)
                ?? throw new LanguageNotConfiguredException(languageId);

            logger.LogInformation("Starting compilation session for language {languageId}", languageId);

            if (IOUtilities.IsValidPath(environmentFolder) is false)
                environmentFolder = await CreateTemporaryFolderAsync(cancellationToken);

            var sourceFilename = Path.Combine(Path.GetDirectoryName(languageConfiguration.Compile.DotnetProjectPath), languageConfiguration.Compile.SourceName);
            var executablePath = Path.Combine(environmentFolder, Path.GetFileNameWithoutExtension(languageConfiguration.Compile.DotnetProjectPath));
            var arguments = languageConfiguration.Compile.Arguments?
                .Select(argument =>
                {
                    if (argument.Contains("{src_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{src_path}", languageConfiguration.Compile.DotnetProjectPath);
                    else if (argument.Contains("{exe_path}", StringComparison.OrdinalIgnoreCase))
                        return argument.Replace("{exe_path}", environmentFolder);

                    return argument;
                });
            await File.WriteAllTextAsync(sourceFilename, source, cancellationToken);

            var startInfo = new ProcessStartInfo
            {
                FileName = languageConfiguration.Compile.Command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = string.Join(" ", arguments ?? Enumerable.Empty<string>())
            };
            var process = new Process { StartInfo = startInfo };

            process.Start();
            await process.WaitForExitAsync(cancellationToken);

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            logger.LogInformation(
                "Libjudger.so process finished. Exit code: {exitCode}, Output: {output}, Error: {error}.",
                process.ExitCode,
                output,
                error);

            return new CompilationResult(new ExecutionResult
            {
                Status = process.ExitCode == 0
                        ? Wrapper.Abstractions.Models.EExecutionResult.Success
                        : Wrapper.Abstractions.Models.EExecutionResult.RuntimeError,
                Error = process.ExitCode == 0
                        ? Wrapper.Abstractions.Models.EExecutionError.NoError
                        : Wrapper.Abstractions.Models.EExecutionError.ExecveFailed
            })
            {
                Output = output,
                Error = error,
                ExecutableFilePath = executablePath
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to .NET compiler for language {languageId}.", languageId);
            throw new CompilationProcessFailedException($"Failed to .NET compiler for language {languageId}.", ex);
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

    private async ValueTask<string> CreateTemporaryFolderAsync(CancellationToken cancellationToken)
    {
        var folder = IOUtilities.CreateTemporaryDirectory();
        await cli.AddPathOwnerAsync(judgeUsers.Compiler.Username, folder, cancellationToken: cancellationToken);
        logger.LogInformation("Created temporary folder: {tempFolder}", folder);

        shouldCleanUpEnvironmentFolder = true;

        return folder;
    }
}