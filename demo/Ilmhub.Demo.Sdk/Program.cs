using System.Text.Json;
using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Models;
using Ilmhub.Judge.Wrapper.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(configure => 
{
    // configure.AddFilter("Ilmhub.Judge.*", LogLevel.Error);
});
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
                Arguments = new string[] { "{src_path}", "-DONLINE_JUDGE", "-O2", "-fmax-errors=3", "-std=c99", "-lm", "-o", "{exe_path}" }
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
        },
        new LanguageConfiguration
        {
            Id = 3,
            Name = "C# (mono)",
            Compile = new CompileConfiguration()
            {
                SourceName = "Main.cs",
                ExecutableName = "Main",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = -1,
                Command = "/usr/bin/mcs",
                Arguments = new string[] { "-out:{exe_path}", "{src_path}" }
            },
            Run = new RunConfiguration()
            {
                Command = "/usr/bin/mono",
                SeccompRule = string.Empty,
                Arguments = new string[] { "{exe_path}" },
                EnvironmentVariables = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                MemoryLimitCheckOnly = true
            }
        },
        new LanguageConfiguration
        {
            Id = 4, 
            Name = "Python (3)",
            Compile = new CompileConfiguration()
            {
                SourceName = "solution.py",
                ExecutableName = "__pycache__/solution.cpython-36.pyc",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = 128 * 1024 * 1024,
                Command = "/usr/bin/python3",
                Arguments = new string[] { "-m", "py_compile", "{src_path}" }
            },
            Run = new RunConfiguration()
            {
                Command = "/usr/bin/python3",
                Arguments = new string[] { "{exe_path}" },
                SeccompRule = "general",
                EnvironmentVariables = new string[] { "PYTHONIOENCODING=UTF-8", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
            }
        },
        new LanguageConfiguration
        {
            Id = 5,
            Name = "Go",
            Compile = new CompileConfiguration()
            {
                SourceName = "main.go",
                ExecutableName = "main",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = 1024 * 1024 * 1024,
                Command = "/usr/bin/go",
                Arguments = new string[] { "build", "-o", "{exe_path}", "{src_path}" },
                EnvironmentVariables = new string[] { "GOCACHE=/tmp", "PATH='$PATH'" }
            },
            Run = new RunConfiguration()
            {
                Command = "{exe_path}",
                SeccompRule = "",
                EnvironmentVariables = new string[] { "GODEBUG=madvdontneed=1", "GOCACHE=off", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                MemoryLimitCheckOnly = true
            }
        },
        new LanguageConfiguration
        {
            Id = 6,
            Name = "Java",
            Compile = new CompileConfiguration()
            {
                SourceName = "Main.java",
                ExecutableName = "Main",
                MaxCpuTime = 3000,
                MaxRealTime = 5000,
                MaxMemory = -1,
                Command = "/usr/bin/javac",
                Arguments = new string[] { "{src_path}", "-d", "{exe_dir}", "-encoding", "UTF8" }
            },
            Run = new RunConfiguration()
            {
                Command = "/usr/bin/java",
                SeccompRule = null,
                Arguments = new string[] { "-cp", "{exe_dir}", "-XX:MaxRAM={max_memory}k", "-Dfile.encoding=UTF-8", "-Djava.security.policy==/etc/java_policy", "-Djava.awt.headless=true", "Main" },
                EnvironmentVariables = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                MemoryLimitCheckOnly = true
            }
        },
    };
});

var app = builder.Build();
var compiler = app.Services.GetRequiredService<ICompiler>();
var cli = app.Services.GetRequiredService<ILinuxCommandLine>();
var languageService = app.Services.GetRequiredService<ILanguageService>();

// var cWarning = @"int main() { printf(""Hello, World!""); return 0; }";
// await Compile(cWarning, 1);

// var cError = @"#include <stdio.h> int main() { printf(""Hello, World!""); return 0 }";
// await Compile(cError, 1);

// var cWarningError = @"int main() { printf(""Hello, World!""); return 0 }";
// await Compile(cWarningError, 1);

// var cFine = @" int main() { printf(""Hello, World!""); return 0; }";
// await Compile(cFine, 1);

// string cppSource = "#include <iostream>\n\nint main() { std::cout << \"Hello, world!\" << std::endl; return 0 }";
// await Compile(cppSource, 2);

// string cSharpSource = "using System; class Program { static void Main() { Console.WriteLine(\"Hello, World!\") } }";
// await Compile(cSharpSource, 3);

// string python3Source = @"print(""Hello, World!"")";
// await Compile(python3Source, 4);

// string goSource = @"package main; import ""fmt""; func main() { fmt.Println(""Hello, World!"") }";
// await Compile(goSource, 5);


string javaSource = "public class Main { public static void main(String[] args) { System.out.println(\"Hello, World!\"); } }";
await Compile(javaSource, languageId: 6);

async ValueTask Compile(string source, int languageId)
{
    var exePath = Path.Combine("/judger", Guid.NewGuid().ToString());
    Directory.CreateDirectory(exePath);
    var compilationResult = await compiler.CompileAsync(
        source: source, 
        languageId: languageId, 
        executableFilePath: exePath,
        CancellationToken.None);
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("OUTPUT");
    Console.WriteLine(JsonSerializer.Serialize(compilationResult, new JsonSerializerOptions() { WriteIndented = true }));
    // Console.WriteLine($"LanguageID: {languageId}");
    // var config = await languageService.GetLanguageConfigurationOrDefaultAsync(languageId);
    // await cli.ChangePathModeAsync("777", Path.Combine(exePath, config.Compile.ExecutableName));
    // await cli.RunCommandAsync(Path.Combine(exePath, config.Compile.ExecutableName), "");
    await cli.RemoveFolderAsync(exePath);
}

