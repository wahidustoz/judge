using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Sdk.Tests.Integration;

public class SpecialJudgeTests
{
    private readonly ServiceProvider provider;

    public SpecialJudgeTests()
        => provider = ServiceCollectionMockProvider.SetupServiceProvider("http://localhost:12358", "123token");
    
    [Fact]
    public async void CSharpCodeSpecialJudgeWithDynamicOutputSucceedsAsync()
    {
        // Given
        string code = @"using System; class Program { static void Main() { DateTime currentTime = DateTime.Now; string formattedTime = currentTime.ToString(""HH:mm:ss""); Console.WriteLine(""Current Time: "" + formattedTime); } }";
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var request = new JudgeSpecialRequest
        {
            Version = Guid.NewGuid().ToString(),
            ShouldReturnOutput = true,
            SourceCode = code,
            SpecialSourceCode = code,
            Configuration = LanguageConfiguration.Defaults[Abstractions.Models.ELanguageType.CSharp],
        };
        // When
        await client.CompileSpecialAsync(request, CancellationToken.None);
        // Then
    }
}
