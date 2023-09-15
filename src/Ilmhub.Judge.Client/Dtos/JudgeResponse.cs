using System.Text.Json.Serialization;
using Ilmhub.Judge.Client.Json;

namespace Ilmhub.Judge.Client.Dtos;

[JsonConverter(typeof(JudgeResponseConverter))]
public class JudgeResponse
{
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage) && TestCases?.Any() is true;
    public IEnumerable<TestCaseResponseDto> TestCases { get; set; }
}

public class TestCaseResponseDto
{
    [JsonPropertyName("cpu_time")]
    public long CpuTime { get; set; }
    [JsonPropertyName("real_time")]
    public long RealTime { get; set; }
    [JsonPropertyName("memory")]
    public long Memory { get; set; }
    [JsonPropertyName("signal")]
    public long Signal { get; set; }
    [JsonPropertyName("exit_code")]
    public long ExitCode { get; set; }
    [JsonPropertyName("error")]
    public long Error { get; set; }
    [JsonPropertyName("result")]
    public long Result { get; set; }
    [JsonPropertyName("test_case")]
    public string TestCase { get; set; }
    [JsonPropertyName("output_md5")]
    public string OutputMd5 { get; set; }
    [JsonPropertyName("output")]
    public string Output { get; set; }
}