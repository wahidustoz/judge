using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class LanguageConfiguration : ILanguageConfiguration
{
    public string Name { get; set; }
    public ICompileConfiguration Compile { get; set; }
    public IRunConfiguration Run { get; set; }

    public static Dictionary<ELanguageType, LanguageConfiguration> Defaults => new()
    {
        {
            ELanguageType.C,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "main.c",
                    ExecutableName = "main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 128 * 1024 * 1024,
                    CompileCommand = "/usr/bin/gcc -DONLINE_JUDGE -O2 -w -fmax-errors=3 -std=c99 {src_path} -lm -o {exe_path}"
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = "c_cpp",
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            }
        },
        {
            ELanguageType.Pascal,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "main.pas",
                    ExecutableName = "main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 128 * 1024 * 1024,
                    CompileCommand = "/usr/bin/fpc -O2 {src_path} -o{exe_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = string.Empty,
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            }
        },
        {
            ELanguageType.Cpp,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "main.cpp",
                    ExecutableName = "main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 128 * 1024 * 1024,
                    CompileCommand = "/usr/bin/g++ -DONLINE_JUDGE -O2 -w -fmax-errors=3 -std=c++11 {src_path} -lm -o {exe_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = "c_cpp",
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            }
        },
        {
            ELanguageType.Java,
            new LanguageConfiguration
            {
                Name = "java",
                Compile = new CompileConfiguration()
                {
                    SourceName = "Main.java",
                    ExecutableName = "Main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = -1,
                    CompileCommand = "/usr/bin/javac {src_path} -d {exe_dir} -encoding UTF8"
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/java -cp {exe_dir} -XX:MaxRAM={max_memory}k -Djava.security.manager -Dfile.encoding=UTF-8 -Djava.security.policy==/etc/java_policy -Djava.awt.headless=true Main",
                    SeccompRule = null,
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            }
        },
        {
            ELanguageType.CSharp,
            new LanguageConfiguration
            {
                Name = "csharp",
                Compile = new CompileConfiguration()
                {
                    SourceName = "Main.cs",
                    ExecutableName = "Main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = -1,
                    CompileCommand = "/usr/bin/mcs -out:{exe_path} {src_path}"
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/mono {exe_path}",
                    SeccompRule = string.Empty,
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            }
        },
        {
            ELanguageType.Python2,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "solution.py",
                    ExecutableName = "solution.pyc",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 128 * 1024 * 1024,
                    CompileCommand = "/usr/bin/python -m py_compile {src_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/python {exe_path}",
                    SeccompRule = "general",
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            }
        },
        {
            ELanguageType.Python3,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "solution.py",
                    ExecutableName = "__pycache__/solution.cpython-36.pyc",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 128 * 1024 * 1024,
                    CompileCommand = "/usr/bin/python3 -m py_compile {src_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/python3 {exe_path}",
                    SeccompRule = "general",
                    Environment = new string[] { "PYTHONIOENCODING=UTF-8", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            }
        },
        {
            ELanguageType.JavaScript,
            new LanguageConfiguration
            {
                Run = new RunConfiguration()
                {
                    ExecutableName = "solution.js",
                    Command = "/usr/bin/node {exe_path}",
                    SeccompRule = "",
                    Environment = new string[] { "NO_COLOR=true", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            }
        },
        {
            ELanguageType.Go,
            new LanguageConfiguration
            {
                Compile = new CompileConfiguration()
                {
                    SourceName = "main.go",
                    ExecutableName = "main",
                    MaxCpuTime = 3000,
                    MaxRealTime = 5000,
                    MaxMemory = 1024 * 1024 * 1024,
                    CompileCommand = "/usr/bin/go build -o {exe_path} {src_path}",
                    Environment = new string[] { "GOCACHE=/tmp" }
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = "",
                    Environment = new string[] { "GODEBUG=madvdontneed=1", "GOCACHE=off", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            }
        }
    };
}