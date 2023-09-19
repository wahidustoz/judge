using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Ilmhub.Judge.Api.Models;

namespace Ilmhub.Judge.Api.Services;

public class RunnerService
{
    private const string JUDGER_PATH = "/usr/lib/judger/libjudger.so";
    private const string ERROR_PATH = "error";
    private const string LOG_PATH = "log";
    private readonly ILogger<RunnerService> logger;

    public RunnerService(ILogger<RunnerService> logger) => this.logger = logger;

    public async ValueTask<RunResult> RunAsync(RunRequest request, CancellationToken cancellationToken = default)
    {
        var tempDirectory = CreateTemporaryFolder();
        logger.LogInformation("Creating temporary folder: {tempDirectory}", tempDirectory);

        var startInfo = new ProcessStartInfo
        {
            FileName = JUDGER_PATH,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = BuildArgumentList(request, tempDirectory)
        };

        logger.LogInformation("Starting process: {startInfo}", startInfo);

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        await process.WaitForExitAsync();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        var processLog = File.ReadAllText(Path.Combine(tempDirectory, LOG_PATH));
        var processError = File.ReadAllText(Path.Combine(tempDirectory, ERROR_PATH));

        if(string.IsNullOrWhiteSpace(processLog) is false)
            logger.LogWarning("Process log:\n{log}", processLog);
        if(string.IsNullOrWhiteSpace(processError) is false)
            logger.LogWarning("Process error:\n{error}", processError);

        logger.LogInformation("Removeing temporary folder: {tempDirectory}", tempDirectory);
        DestroyTemporaryFolder(tempDirectory);

        logger.LogInformation("Process exit code: {exitCode}", process.ExitCode);
        logger.LogInformation("Process output:\n{output}", output);

        try
        {
            var result = JsonSerializer.Deserialize<RunResult>(output);
            return result;
        }
        catch(Exception e)
        {
            logger.LogWarning(e, "Failed to deserialize process output {output}.", output);
            throw;
        }
    }

    private void DestroyTemporaryFolder(string tempDirectory) => Directory.Delete(tempDirectory, true);

    private string CreateTemporaryFolder()
    {
        var tempFolder = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())); 
        Directory.CreateDirectory(tempFolder);
        var errorFile = File.Create(Path.Combine(tempFolder, ERROR_PATH));
        var logFIle = File.Create(Path.Combine(tempFolder, LOG_PATH));

        errorFile.Close();
        logFIle.Close();

        return tempFolder;
    }

    private string BuildArgumentList(RunRequest request, string tempDirectory)
    {
        StringBuilder builder = new StringBuilder();
        AppendSingleArgument(builder, "exe_path", request.ExecutablePath);
        AppendSingleArgument(builder, "input_path", request.InputPath);
        AppendSingleArgument(builder, "output_path", request.OutputPath);
        AppendSingleArgument(builder, "error_path", Path.Combine(tempDirectory, ERROR_PATH));
        AppendSingleArgument(builder, "log_path", Path.Combine(tempDirectory, LOG_PATH));
        AppendSingleArgument(builder, "seccomp_rule_name", request.SeccompRuleName);
        
        // below values have defualt value so we dont need to null-check
        builder.Append($" --max_cpu_time={request.CpuTime}");
        builder.Append($" --max_real_time={request.RealTime}");
        builder.Append($" --max_memory={request.Memory}");
        builder.Append($" --memory_limit_check_only={(request.MemoryLimitCheckOnly ? 1 : 0)}");
        builder.Append($" --max_stack={request.Stack}");
        builder.Append($" --max_process_number={request.ProcessNumber}");
        builder.Append($" --max_output_size={request.OutputSize}");
        builder.Append($" --uid={request.Uid}");
        builder.Append($" --gid={request.Gid}");

        builder.Append($" --env=\"PATH={Environment.GetEnvironmentVariable("PATH")}\"");

        if(request.Environments?.Any() is true)
            foreach(var env in request.Environments)
                AppendSingleArgument(builder, "env", env);

        if(request.Arguments?.Any() is true)
            foreach(var arg in request.Arguments)
                AppendSingleArgument(builder, "args", arg);
        
        logger.LogInformation("Libjudger arguments: {arguments}", builder.ToString());

        return builder.ToString();
    }

    private void AppendSingleArgument(StringBuilder builder, string key, string value) => 
        if(string.IsNullOrWhiteSpace(value) is false) 
            builder.Append($" --{key}={value}");
}