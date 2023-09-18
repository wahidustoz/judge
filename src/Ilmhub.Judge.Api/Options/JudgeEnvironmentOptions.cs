using Ilmhub.Judge.Api.Models;

namespace Ilmhub.Judge.Api.Options;

public class JudgeEnvironmentOptions
{
    public EnvironmentUser Compiler { get; set; }
    public EnvironmentUser Runner { get; set; }
    public string RootPath { get; set; }
}