using System.Text.Json.Serialization;
using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Dtos;

public class JudgeRequestDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("output")]
    public bool ShouldReturnOutput { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("src")]
    public string SourceCode { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("language_config")]
    public LanguageConfigurationDto LanguageConfiguration { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("max_cpu_time")]
    public long MaxCpuTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("max_memory")]
    public long MaxMemory { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("test_case")]
    public IEnumerable<TestCaseDto> TestCases { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("test_case_id")]
    public string TestCaseId { get; set; }
}

public class LanguageConfigurationDto
{
    public LanguageConfigurationDto(ILanguageConfiguration languageConfiguration)
    {
        Name = languageConfiguration.Name;
        Compile = new CompileConfigurationDto(languageConfiguration.Compile);
        Run = new RunConfigurationDto(languageConfiguration.Run);
    }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("compile")]
    public CompileConfigurationDto Compile { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("run")]
    public RunConfigurationDto Run { get; set; }
}

public class CompileConfigurationDto
{
    public CompileConfigurationDto(ICompileConfiguration model)
    {
        SourceName = model.SourceName;
        ExecutableName = model.ExecutableName;
        MaxCpuTime = model.MaxCpuTime;
        MaxMemory = model.MaxMemory;
        MaxRealTime = model.MaxRealTime;
        CompileCommand = model.CompileCommand;
        Environment = model.Environment;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("src_name")]
    public string SourceName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("exe_name")]
    public string ExecutableName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("max_cpu_time")]
    public int MaxCpuTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("max_real_time")]
    public int MaxRealTime { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("max_memory")]
    public int MaxMemory { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("compile_command")]
    public string CompileCommand { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("env")]
    public IEnumerable<string> Environment { get; set; }
}

public class RunConfigurationDto
{
    public RunConfigurationDto(IRunConfiguration model)
    {
        ExecutableName = model.ExecutableName;
        Command = model.Command;
        SeccompRule = model.SeccompRule;
        Environment = model.Environment;
        MemoryLimitCheckOnly = model.MemoryLimitCheckOnly ? 1 : 0;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("exe_name")]
    public string ExecutableName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("command")]
    public string Command { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("seccomp_rule")]
    public string SeccompRule { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull), JsonPropertyName("env")]
    public IEnumerable<string> Environment { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault), JsonPropertyName("memory_limit_check_only")]
    public int MemoryLimitCheckOnly { get; set; }
}

public class TestCaseDto
{
    public TestCaseDto(ITestCase model)
    {
        Input = model.Input;
        Output = model.Output;    
    }

    [JsonPropertyName("input")]
    public string Input { get; set; }
    [JsonPropertyName("output")]
    public string Output { get; set; }
}