using System.Net.Http.Json;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Sdk.Extensions;
using Ilmhub.Judge.Sdk.Models;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;

namespace Ilmhub.Judge.Sdk;

public class JudgeClient : IJudgeClient
{
    private readonly ResiliencePipeline pipeline;
    private readonly ILogger<JudgeClient> logger;
    private readonly HttpClient httpClient;

    public JudgeClient(
        ILogger<JudgeClient> logger,
        HttpClient httpClient,
        ResiliencePipelineProvider<string> pipelineProvider)
    {
        this.pipeline = pipelineProvider.GetPipeline(nameof(JudgeClient));
        this.logger = logger;
        this.httpClient = httpClient;
    }
    public async ValueTask<Guid> AddTestCasesAsync(
        IEnumerable<TestCase> testCases,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await pipeline.ExecuteAsync(async context =>
            {
                var response = await httpClient.PostAsJsonAsync("/v1/testcases", testCases, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Guid>();
            });
        }
        catch (HttpRequestException ex) when (ex.IsClientError())
        {
            logger.LogTrace(ex, "JudgeClient: request failed due to client error.");
            throw new JudgeClientRequestValidationException(ex);
        }
        catch (Exception ex)
        {
            logger.LogException(ex);
            throw new JudgeClientException(ex);
        }
    }

    public ValueTask<Language> GetLanguagesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<JudgeResult> JudgeAsync(string sourceCode, int languageId, Guid testCasesId, long? maxCpu = null, long? maxMemory = null, bool? useStrictMode = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<JudgeResult> JudgeAsync(string sourceCode, int languageId, IEnumerable<TestCase> testCases, long? maxCpu = null, long? maxMemory = null, bool? useStrictMode = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<RunResult> RunAsync(string sourceCode, int languageId, IEnumerable<string> inputs, long? maxCpu = null, long? maxMemory = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask<RunResult> RunAsync(string sourceCode, int languageId, Guid testCasesId, long? maxCpu = null, long? maxMemory = null, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}