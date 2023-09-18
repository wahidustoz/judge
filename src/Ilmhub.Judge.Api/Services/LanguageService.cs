namespace Ilmhub.Judge.Api.Services;

public class LanguageService
{
    public async ValueTask<bool> LanguageExistsAsync(int languageId, CancellationToken cancellationToken = default)
        => (await GetSupportedLanguageDefaultConfigurationsAsync(cancellationToken)).Any(x => x.Id == languageId);
    
    public async ValueTask<LanguageConfiguration> GetLanguageConfigurationAsync(int languageId, CancellationToken cancellationToken = default)
        => (await GetSupportedLanguageDefaultConfigurationsAsync(cancellationToken)).FirstOrDefault(x => x.Id == languageId);

    public ValueTask<IEnumerable<LanguageConfiguration>> GetSupportedLanguageDefaultConfigurationsAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult<IEnumerable<LanguageConfiguration>>(new List<LanguageConfiguration>
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
                    CompileCommand = "/usr/bin/gcc -DONLINE_JUDGE -O2 -w -fmax-errors=3 -std=c99 {src_path} -lm -o {exe_path}"
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = "c_cpp",
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
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
                    CompileCommand = "/usr/bin/g++ -DONLINE_JUDGE -O2 -w -fmax-errors=3 -std=c++11 {src_path} -lm -o {exe_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "{exe_path}",
                    SeccompRule = "c_cpp",
                    Environment = new string[] { "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            },
            new LanguageConfiguration
            {
                Id = 3,
                Name = "C#",
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
                    CompileCommand = "/usr/bin/python3 -m py_compile {src_path}",
                },
                Run = new RunConfiguration()
                {
                    Command = "/usr/bin/python3 {exe_path}",
                    SeccompRule = "general",
                    Environment = new string[] { "PYTHONIOENCODING=UTF-8", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" }
                }
            },
            new LanguageConfiguration
            {
                Id = 5,
                Name = "JavaScript",
                Run = new RunConfiguration()
                {
                    ExecutableName = "solution.js",
                    Command = "/usr/bin/node {exe_path}",
                    SeccompRule = "",
                    Environment = new string[] { "NO_COLOR=true", "LANG=en_US.UTF-8", "LANGUAGE=en_US:en", "LC_ALL=en_US.UTF-8" },
                    MemoryLimitCheckOnly = true
                }
            },
            new LanguageConfiguration
            {
                Id = 6,
                Name = "Go",
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
            },
            new LanguageConfiguration
            {
                Id = 7,
                Name = "Python (2)",
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
            },
            new LanguageConfiguration
            {
                Id = 7,
                Name = "Java",
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
            },
            new LanguageConfiguration
            {
                Id = 8,
                Name = "Pascal",
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
        });
}

public class LanguageConfiguration
{
    public int Id { get; set; }
    public string Name { get; set; }
    public CompileConfiguration Compile { get; set; }
    public RunConfiguration Run { get; set; }
}

public class CompileConfiguration
{
    public string SourceName { get; set; }
    public string ExecutableName { get; set; }
    public int MaxCpuTime { get; set; }
    public int MaxRealTime { get; set; }
    public int MaxMemory { get; set; }
    public string CompileCommand { get; set; }
    public IEnumerable<string> Environment { get; set; }
}

public class RunConfiguration
{
    public string ExecutableName { get; set; }
    public string Command { get; set; }
    public string SeccompRule { get; set; }
    public IEnumerable<string> Environment { get; set; }
    public bool MemoryLimitCheckOnly { get; set; }
}