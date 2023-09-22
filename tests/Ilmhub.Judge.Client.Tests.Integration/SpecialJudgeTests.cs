using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Sdk.Tests.Integration;

public class CompileSpecialTests
{
    private readonly ServiceProvider provider;

    public CompileSpecialTests()
        => provider = ServiceCollectionMockProvider.SetupServiceProvider("http://localhost:12358", "123token");
    
    [Fact]
    public async void PythonCompileSpecialSucceedsAsync()
    {
        // Given
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var request = new CompileSpecialRequest
        {
            Version = "3",
            Configuration = LanguageConfiguration.Defaults[Abstractions.Models.ELanguageType.Python3].Compile,
            SourceCode = "import time; current_time = time.localtime(); time_string = time.strftime('%H:%M:%S', current_time); print('Current time:', time_string)"
        };
        // When
        await client.CompileSpecialAsync(request, CancellationToken.None);
        // Then
    }

    [Fact]
    public async void C_CompileSpecialFailsForInvalidSourceAsync()
    {
        // Given
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var request = new CompileSpecialRequest
        {
            Version = "3",
            Configuration = LanguageConfiguration.Defaults[Abstractions.Models.ELanguageType.C].Compile,
            // missing ; at the end
            SourceCode = "#include <stdio.h>\n#include <time.h>\n\nint main() {\n    time_t currentTime;\n    struct tm *localTime;\n    time(&currentTime);\n    localTime = localtime(&currentTime);\n    char timeString[9];\n    strftime(timeString, sizeof(timeString), \"%T\", localTime);\n    printf(\"Current time: %s\\n\", timeString);\n    return 0 }"
        };
        // When
        var compileTask = client.CompileSpecialAsync(request, CancellationToken.None).AsTask();
        // Then
        await Assert.ThrowsAsync<CompileSpecialErrorException>(() => compileTask);
    }
}
