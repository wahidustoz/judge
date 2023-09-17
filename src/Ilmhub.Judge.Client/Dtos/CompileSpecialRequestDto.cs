using System.Text.Json.Serialization;
using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Dtos;

public class CompileSpecialRequestDto
{
    public CompileSpecialRequestDto(ICompileSpecialRequest model)
    {
        SourceCode = model.SourceCode;
        Version = model.Version;
        Configuration = new CompileConfigurationDto(model.Configuration);
    }
    
    [JsonPropertyName("src")]
    public string SourceCode { get; set; }
    [JsonPropertyName("spj_version")]
    public string Version { get; set; }
    [JsonPropertyName("spj_compile_config")]
    public CompileConfigurationDto Configuration { get; set; }
}
