using System.IO.Compression;
using System.Text.Json;
using Ilmhub.Judge.Sdk.Abstraction.Models;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Sdk.Models;
using Ilmhub.Judge.Wrapper;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Utilities;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public class Judger : IJudger
{
    private const string TESTCASES_FOLDER = "testcases";
    private const string INPUT_EXTENSION = ".in";
    private const string OUTPUT_EXTENSION = ".out";
    private readonly ILogger<IJudger> logger;
    private readonly ILinuxCommandLine cli;
    private readonly ICompilationHandler compiler;
    private readonly IRunner runner;
    private readonly IIlmhubJudgeOptions options;
    private readonly IJudgeUsersOption judgeUsers;

    public Judger(
        ILogger<IJudger> logger,
        ILinuxCommandLine cli,
        ICompilationHandler compiler,
        IRunner runner,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.cli = cli;
        this.compiler = compiler;
        this.runner = runner;
        this.options = options;
        this.judgeUsers = options.SystemUsers;
    }

    public bool TestCaseExists(Guid testCaseId) => IOUtilities.IsValidPath(GetTestCaseFolder(testCaseId));
    public string GetTestCaseFolder(Guid testCaseId) => Path.Combine(options.RootFolder, TESTCASES_FOLDER, testCaseId.ToString());

    public async ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source,
        IEnumerable<ITestCase> testCases,
        bool? usestrictMode = default,
        long maxCpu = -1,
        long maxMemory = -1,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        var shouldDestroyTemporaryFolder = false;
        try
        {
            if(IOUtilities.IsValidPath(environmentFolder) is false)
            {
                environmentFolder = IOUtilities.CreateTemporaryDirectory();
                shouldDestroyTemporaryFolder = true;
            }

            var testcasesFolder = Path.Combine(environmentFolder, TESTCASES_FOLDER);
            Directory.CreateDirectory(testcasesFolder);
            await WriteTestCasesAsync(testcasesFolder, testCases, cancellationToken);
            return await JudgeInternalAsync(languageId, source, maxCpu, maxMemory, testcasesFolder, usestrictMode, environmentFolder, cancellationToken);
        }
        catch(Exception ex) when (ex is not JudgeFailedException)
        {
            logger.LogError(ex, "Judge operation failed.");
            throw new JudgeFailedException("Judge failed with unknown issue.", ex);
        }
        finally
        {
            if(shouldDestroyTemporaryFolder)
            {
                logger.LogInformation("Removing temporary judge environment folder: {folder}", environmentFolder);
                await cli.RemoveFolderAsync(environmentFolder, cancellationToken);
            }
        }
    }

    public async ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source,
        Guid testCaseId,
        bool? usestrictMode = default,
        long maxCpu = -1,
        long maxMemory = -1,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        if(TestCaseExists(testCaseId) is false)
            throw new TestCaseNotFoundException(testCaseId);

        var testCasesFolder = GetTestCaseFolder(testCaseId);
        return await JudgeInternalAsync(languageId, source, maxCpu, maxMemory, testCasesFolder, usestrictMode, environmentFolder, cancellationToken);
    }

    async ValueTask<IJudgeResult> JudgeInternalAsync(
        int languageId,
        string source,
        long maxCpu,
        long maxMemory,
        string testCasesFolder,
        bool? usestrictMode = default,
        string environmentFolder = default,
        CancellationToken cancellationToken = default)
    {
        var shouldDestroyTemporaryFolder = false;
        if(IOUtilities.IsValidPath(environmentFolder) is false)
        {
            environmentFolder = IOUtilities.CreateTemporaryDirectory();
            shouldDestroyTemporaryFolder = true;
        }

        try
        {
            var compilationResult = await compiler.CompileAsync(
                source: source,
                languageId: languageId,
                environmentFolder: environmentFolder,
                cancellationToken: cancellationToken);

            if(compilationResult.IsSuccess is false)
            {
                logger.LogError(
                    "Compilation failed for language {id}. Result: {result}. ABORTING....",
                    languageId,
                    JsonSerializer.Serialize(compilationResult, new JsonSerializerOptions { WriteIndented = true }));
                return new JudgeResult(compilationResult);
            }

            var testCases = EnumerateTestCases(testCasesFolder);
            await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, testCasesFolder, true, cancellationToken);
            await cli.ChangePathModeAsync(LinuxCommandLine.EXECUTE_MODE, testCasesFolder, true, cancellationToken);

            var testCaseResults = new List<TestCaseResult>();
            foreach(var (InputFilePath, OutputFilePath, Id) in testCases)
            {
                var outputTask = IOUtilities.GetAllTextOrDefaultAsync(OutputFilePath, cancellationToken);
                IRunnerResult runnerResult = default;
                if(string.IsNullOrWhiteSpace(InputFilePath))
                {
                    runnerResult = await runner.RunAsync(
                        languageId,
                        compilationResult.ExecutableFilePath,
                        maxCpu,
                        maxMemory,
                        environmentFolder,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    runnerResult = await runner.RunAsync(
                        languageId,
                        InputFilePath,
                        compilationResult.ExecutableFilePath,
                        maxCpu,
                        maxMemory,
                        environmentFolder,
                        cancellationToken);
                }

                if(runnerResult.IsSuccess is false)
                {
                    logger.LogError(
                        "Runner failed for language {id} at test case {testcaseId}. Result: {result}. ABORTING....",
                        languageId,
                        Id,
                        JsonSerializer.Serialize(compilationResult, new JsonSerializerOptions { WriteIndented = true }));
                }

                testCaseResults.Add(new TestCaseResult(Id, await outputTask, runnerResult, usestrictMode));
            }

            logger.LogInformation("Completed Judge process 🎉");
            return new JudgeResult(compilationResult, testCaseResults);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Judge operation failed.");
            throw new JudgeFailedException("Judge failed with unknown issue.", ex);
        }
        finally
        {
            if(shouldDestroyTemporaryFolder)
            {
                logger.LogInformation("Removing temporary judge environment folder: {folder}", environmentFolder);
                await cli.RemoveFolderAsync(environmentFolder, cancellationToken);
            }
        }
    }

    private async ValueTask WriteTestCasesAsync(string testcasesFolder, IEnumerable<ITestCase> testCases, CancellationToken cancellationToken)
    {
        foreach(var testCase in testCases)
        {
            if(string.IsNullOrWhiteSpace(testCase.Input) is false)
                await File.WriteAllTextAsync(
                    Path.Combine(testcasesFolder, $"{testCase.Id}{INPUT_EXTENSION}"),
                    testCase.Input,
                    cancellationToken);
            if(string.IsNullOrWhiteSpace(testCase.Output) is false)
                await File.WriteAllTextAsync(
                    Path.Combine(testcasesFolder, $"{testCase.Id}{OUTPUT_EXTENSION}"),
                    testCase.Output,
                    cancellationToken);
        }

        logger.LogInformation("Finished writing {count} test cases.", testCases.Count());
    }

    private IEnumerable<(string InputFilePath, string OutputFilePath, string Id)> EnumerateTestCases(string testCasesFolder)
    {
        var inputFiles = Directory.EnumerateFiles(testCasesFolder, $"*{INPUT_EXTENSION}");
        var outputFiles = Directory.EnumerateFiles(testCasesFolder, $"*{OUTPUT_EXTENSION}");

        logger.LogInformation(
            "Enumerated Testcases. Input files: {inputCount}, Output files: {outputCount}",
            inputFiles.Count(),
            outputFiles.Count());

        var dictionary = inputFiles.ToDictionary(path => Path.GetFileNameWithoutExtension(path));
        foreach(var outputFile in outputFiles)
        {
            var key = Path.GetFileNameWithoutExtension(outputFile);
            if(dictionary.ContainsKey(key))
            {
                var inputFile = dictionary[key];
                dictionary.Remove(key);
                yield return (inputFile, outputFile, key);
            }
            else
                yield return (string.Empty, outputFile, key);
        }

        foreach(var item in dictionary)
            yield return (item.Value, string.Empty, item.Key);
    }

    public async ValueTask<Guid> CreateTestCaseAsync(IEnumerable<ITestCase> testCases, CancellationToken cancellationToken = default)
    {
        var testCaseId = Guid.NewGuid();
        var testCasesFolder = GetTestCaseFolder(testCaseId);
        var testCasesDirectory = Directory.CreateDirectory(testCasesFolder);
        await WriteTestCasesAsync(testCasesDirectory.ToString(), testCases, cancellationToken);
        return testCaseId;
    }

    public Guid CreateTestCaseFromZipArchive(Stream zipStream)
    {
        var testCaseId = Guid.NewGuid();
        var testCasesFolder = GetTestCaseFolder(testCaseId);

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Read))
        {
            var testCasesDirectory = Directory.CreateDirectory(testCasesFolder);
            foreach (var entry in zip.Entries)
                if (entry.Name.EndsWith(".in") || entry.Name.EndsWith(".out"))
                    entry.ExtractToFile(Path.Combine(testCasesFolder, entry.Name), true);
        }
        return testCaseId;
    }
}