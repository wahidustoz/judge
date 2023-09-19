using System.Text.Json.Serialization;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Sdk.Dtos;

public class JudgeSpecialRequestDto : JudgeRequestDto
{
    public JudgeSpecialRequestDto(IJudgeSpecialRequest model) 
        : base(new JudgeRequest
        {
            SourceCode = model.SourceCode,
            TestCaseId = model.TestCaseId,
            TestCases = model.TestCases,
            LanguageConfiguration = model.LanguageConfiguration,
            MaxCpuTime = model.MaxCpuTime,
            MaxMemory = model.MaxMemory,
            ShouldReturnOutput = model.ShouldReturnOutput
        })
    {
        SpecialSourceCode = model.SpecialSourceCode;
        Version = model.Version;
        SpecialCompileConfiguration = new CompileConfigurationDto(model.SpecialCompileConfiguration);
        SpecialJudgeConfiguration = new RunConfigurationDto(model.SpecialJudgeConfiguration);

    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("spj_src")]
    public string SpecialSourceCode { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("spj_version")]
    public string Version { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("spj_compile_config")]
    public CompileConfigurationDto SpecialCompileConfiguration { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("spj_config")]
    public RunConfigurationDto SpecialJudgeConfiguration { get; set; }
}
