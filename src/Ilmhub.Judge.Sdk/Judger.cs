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
    private readonly ICompiler compiler;
    private readonly IRunner runner;
    private readonly IJudgeUsersOption judgeUsers;

    public Judger(
        ILogger<IJudger> logger,
        ILinuxCommandLine cli,
        ICompiler compiler,
        IRunner runner,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.cli = cli;
        this.compiler = compiler;
        this.runner = runner;
        this.judgeUsers = options.SystemUsers;
    }

    public async ValueTask<IJudgeResult> JudgeAsync(
        int languageId,
        string source, 
        long maxCpu,
        long maxMemory,
        IEnumerable<ITestCase> testCases, 
        CancellationToken cancellationToken = default)
    {
        var judgeEnvironmentFolder = IOUtilities.CreateTemporaryDirectory();
        try
        {
            var testcasesFolder = Path.Combine(judgeEnvironmentFolder, TESTCASES_FOLDER);
            Directory.CreateDirectory(testcasesFolder);
            await WriteTestCasesAsync(testcasesFolder, testCases, cancellationToken);
            return await JudgeAsync(
                languageId, 
                source, 
                maxCpu, 
                maxMemory, 
                testcasesFolder, 
                judgeEnvironmentFolder,
                cancellationToken);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Judge operation failed.");
            throw new JudgeFailedException("Judge failed with unknown issue.", ex);
        }
        finally
        {
            logger.LogInformation("Removing temporary judge environment folder: {folder}", judgeEnvironmentFolder);
            await cli.RemoveFolderAsync(judgeEnvironmentFolder, cancellationToken);
        }
    }

    public async ValueTask<IJudgeResult> JudgeAsync(
        int languageId, 
        string source, 
        long maxCpu, 
        long maxMemory, 
        string testCasesFolder,
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
            await GrantRunnerPermissionToTestCasesAsync(testCasesFolder, testCases, cancellationToken);

            var testCaseResults = new List<TestCaseResult>();
            foreach(var testCase in testCases)
            {
                IRunnerResult runnerResult = default;
                if(string.IsNullOrWhiteSpace(testCase.InputFilePath))
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
                        testCase.InputFilePath, 
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
                        testCase.Id, 
                        JsonSerializer.Serialize(compilationResult, new JsonSerializerOptions { WriteIndented = true }));
                }

                testCaseResults.Add(new TestCaseResult(testCase.Id, runnerResult));
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

    private async ValueTask GrantRunnerPermissionToTestCasesAsync(string testCasesFolder, IEnumerable<(string InputFilePath, string OutputFilePath, string Id)> testCases, CancellationToken cancellationToken)
    {
        await cli.AddPathOwnerAsync(judgeUsers.Runner.Username, testCasesFolder, true, cancellationToken);
        await cli.ChangePathModeAsync(LinuxCommandLine.EXECUTE_MODE, testCasesFolder, true, cancellationToken);

        // TODO: might have to add permissions to input files otherwise remove this method
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
}