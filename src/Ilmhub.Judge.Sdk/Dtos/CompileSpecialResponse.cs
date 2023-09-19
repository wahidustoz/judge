using System.Text.Json.Serialization;

namespace Ilmhub.Judge.Sdk.Dtos;

public class CompileSpecialResponse
{
    [JsonPropertyName("data")]
    public string Data { get; set; }
    [JsonPropertyName("err")]
    public string ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrWhiteSpace(ErrorMessage);
}