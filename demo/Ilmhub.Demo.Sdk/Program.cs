using System.Runtime.CompilerServices;
using System.Text.Json;
using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIlmhubJudge(options =>
{
    options.SystemUsers = new JudgeUsersOption
    {
        Compiler = new JudgeSystemUser("testcompiler", 2000, 2000),
        Runner = new JudgeSystemUser("testrunner", 2001, 2001)
    };
    options.LanguageConfigurations = new List<ILanguageConfiguration>
    {
        new LanguageConfiguration
        {
            Id = 1,
            Name = "C",
            Compile = new CompileConfiguration()
            {
                SourceName = "main.c",
                ExecutableName = "main",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = 128 * 1024 * 1024,
                Command = "/usr/bin/gcc",
                Arguments = new string[] { "{src_path}", "-DONLINE_JUDGE", "-O2", "-w", "-fmax-errors=3", "-std=c99", "-lm", "-o", "{exe_path}" }
            },
            Run = new RunConfiguration()
            {
                Command = "{exe_path}",
                SeccompRule = "c_cpp"
            }
        }, 
        new LanguageConfiguration
        {
            Id = 2,
            Name = "C++",
            Compile = new CompileConfiguration()
            {
                SourceName = "main.cpp",
                ExecutableName = "main",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = 128 * 1024 * 1024,
                Command = "/usr/bin/g++",
                Arguments = new string[] { "{src_path}", "-DONLINE_JUDGE", "-O2", "-w", "-fmax-errors=3", "-std=c++11", "-lm", "-o", "{exe_path}" }
            },
            Run = new RunConfiguration()
            {
                Command = "{exe_path}",
                SeccompRule = "c_cpp",
                EnvironmentVariables = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
            }
        }
    };
});

var app = builder.Build();
var compiler = app.Services.GetRequiredService<ICompiler>();

var c_source = @"int main() { printf(""Hello, World!""); return 0; }";
// await Compile(c_source, 1);

var cpp_source = @"#include <iostream>\nint main() { std::cout << ""Hello, World!""; return 0; }";
await Compile(cpp_source, 2);

async ValueTask Compile(string source, int languageId)
{
    var exePath = Path.Combine("/judger", Guid.NewGuid().ToString());
    Directory.CreateDirectory(exePath);
    var compilationResult = await compiler.CompileAsync(
        source: source, 
        languageId: 1, 
        executableFilePath: exePath,
        CancellationToken.None);
    Console.WriteLine(JsonSerializer.Serialize(compilationResult));
    Console.WriteLine(exePath);
}

