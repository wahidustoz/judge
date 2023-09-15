using System.Net.Http.Json;
using System.Text.Json;
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

    public async ValueTask<IJudgeResult> JudgeAsync(IJudgeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(
            requestUri: "/judge", 
            value: new JudgeRequestDto(request),
            cancellationToken: cancellationToken);
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseDto = JsonSerializer.Deserialize<JudgeResponse>(responseString);

        if(responseDto.IsSuccess is false)
            throw new JudgeFailedException(responseDto);

        return new JudgeResult
        {
            ErrorMessage = responseDto.ErrorMessage,
            TestCases = responseDto.TestCases.Select(tc => new TestCaseResult
            {
                CpuTime = tc.CpuTime,
                RealTime = tc.RealTime,
                Memory = tc.Memory,
                Signal = tc.Signal,
                ExitCode = tc.ExitCode,
                Error = tc.Error,
                Result = tc.Result,
                TestCase = tc.TestCase,
                OutputMd5 = tc.OutputMd5,
                Output = tc.Output
            })
        };
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
