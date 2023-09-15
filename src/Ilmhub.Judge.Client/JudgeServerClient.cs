using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ilmhub.Judge.Client.Abstractions;
using Ilmhub.Judge.Client.Abstractions.Models;
using Ilmhub.Judge.Client.Dtos;
using Ilmhub.Judge.Client.Exceptions;
using Ilmhub.Judge.Client.Models;

namespace Ilmhub.Judge.Client;

public class JudgeServerClient : IJudgeServerClient
{
    private readonly HttpClient client;

    public JudgeServerClient(HttpClient client) => this.client = client;

    public async ValueTask<string> JudgeAsync(
        string sourceCode, 
        ILanguageConfiguration languageConfiguration, 
        long maxCpuTime,
        long maxMemory,
        IEnumerable<ITestCase> testCases = default, 
        string testCaseId = null, 
        CancellationToken cancellationToken = default)
    {
        var request = new JudgeRequestDto
        {
            SourceCode = sourceCode,
            MaxCpuTime = maxCpuTime,
            MaxMemory = maxMemory,
            TestCaseId = testCaseId,
            TestCases = testCases.Select(t => new TestCaseDto(t)),
            LanguageConfiguration = new LanguageConfigurationDto(languageConfiguration)
        };

        var requestString = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var response = await client.PostAsJsonAsync(
            requestUri: "/judge", 
            value: request,
            cancellationToken: cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async ValueTask<IServerInfo> PingAsync(CancellationToken cancellationToken = default)
    {
        var repsonse = await client.PostAsJsonAsync("/ping", new { }, cancellationToken);
        var pingResponseDto = JsonSerializer.Deserialize<PingResponse>(await repsonse.Content.ReadAsStringAsync(cancellationToken));

        if(pingResponseDto.IsSuccess is false)
            throw new PingFailedException(pingResponseDto);

        return new ServerInfo
        {
            Hostname = pingResponseDto.Data.Hostname,
            Cpu = pingResponseDto.Data.Cpu,
            CpuCore = pingResponseDto.Data.CpuCore,
            JudgerVersion = pingResponseDto.Data.JudgerVersion,
            Memory = pingResponseDto.Data.Memory
        };
    }
}
