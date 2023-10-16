using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ilmhub.Judge.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Sdk.Compiler.Tests.Integration;

public class RunLanguagesTests
{
    private readonly IServiceProvider provider;

    public RunLanguagesTests()
    {
        this.provider = MockProvider.SetupServiceProvider();
    }

    [Theory]
    [ClassData(typeof(LanguageTheoryData))]
    public async Task RunSourceCodeAsync(string languageName, int languageId, string validSource)
    {
        var compiler = provider.GetRequiredService<ICompiler>();
        var runner = provider.GetRequiredService<IRunner>();
        var languageService = provider.GetRequiredService<ILanguageService>();
        var config = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId);
        
        var root = Directory.CreateDirectory(Path.Combine("/judger", Guid.NewGuid().ToString())).FullName;
        var input = "1 2";
        var expectedOutputMD5 = "2562f2761146dff0ff2b37fb51de5f27"; // GetMD5Hash(expectedOutput);
 
        Console.WriteLine($"Starting test compile for {languageName} with valid source.");
        var validCompilationResult = await compiler.CompileAsync(
            source: validSource,
            languageId: languageId,
            environmentFolder: root);
        Console.WriteLine("Compilation result: " + JsonSerializer.Serialize(validCompilationResult, new JsonSerializerOptions() { WriteIndented = true }));
        Assert.True(validCompilationResult.IsSuccess);
        Assert.NotEmpty(validCompilationResult.ExecutableFilePath);
        Assert.True(validCompilationResult.IsSuccess);

        Console.WriteLine($"Starting test run for {languageName} with valid source.");
        var runResult = await runner.RunAsync(
            languageId: languageId, 
            input: input, 
            executableFilename: validCompilationResult.ExecutableFilePath, 
            maxCpu: 3000, 
            maxMemory: -1, 
            environmentFolder: root, 
            cancellationToken: CancellationToken.None);
        Console.WriteLine("Run result: " + JsonSerializer.Serialize(runResult, new JsonSerializerOptions() { WriteIndented = true }));
        Assert.True(runResult.IsSuccess);
        Assert.NotEmpty(runResult.Output);
        Assert.True(string.Equals(expectedOutputMD5, runResult.OutputMd5, StringComparison.InvariantCultureIgnoreCase));

        // Console.WriteLine($"Starting test for {languageName} with invalid source.");
        // var failingResult = await compiler.CompileAsync(invalidSource, languageId, "C:\\", CancellationToken.None);
        // Console.WriteLine("Failing result: " + JsonSerializer.Serialize(failingResult, new JsonSerializerOptions() { WriteIndented = true }));
        // Assert.True(failingResult.IsSuccess is false);
    }
}