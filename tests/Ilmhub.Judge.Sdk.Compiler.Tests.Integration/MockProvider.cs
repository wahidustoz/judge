using Microsoft.AspNetCore.Builder;
using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Options;
using Ilmhub.Judge.Models;

namespace Ilmhub.Judge.Sdk.Compiler.Tests.Integration;

public static class MockProvider
{
    public static IServiceProvider SetupServiceProvider()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddIlmhubJudge(ConfigureOptions);
        // builder.Services.AddLogging(config => config.AddFilter("Ilmhub.Judge.*", LogLevel.Trace));

        var app = builder.Build();

        return app.Services;
    }

    private static void ConfigureOptions(IIlmhubJudgeOptions options)
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
                    Arguments = new string[] { "{src_path}", "-DONLINE_JUDGE", "-O2", "-fmax-errors=3", "-std=c++11", "-lm", "-o", "{exe_path}" }
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
                    // MaxCpuTime = 3000,
                    // MaxRealTime = 5000,
                    // MaxMemory = -1,
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
                    Arguments = new string[] { "{src_path}", "-d", "{exe_dir}" }
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/java",
                    SeccompRule = null,
                    Arguments = new string[] { "-cp", "{exe_dir}", "-Djava.awt.headless=true", "{exe_path}" },
                    EnvironmentVariables = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            },
        };
    }

}
