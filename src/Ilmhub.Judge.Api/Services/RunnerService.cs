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

        logger.LogInformation("Process log:\n{log}", processLog);
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
        var tempFolder = Directory.CreateTempSubdirectory();
        var errorFile = File.Create(Path.Combine(tempFolder.FullName, ERROR_PATH));
        var logFIle = File.Create(Path.Combine(tempFolder.FullName, LOG_PATH));

        errorFile.Close();
        logFIle.Close();

        return tempFolder.FullName;
    }

    private string BuildArgumentList(RunRequest request, string tempDirectory)
    {

        StringBuilder builder = new StringBuilder();
        builder.Append($" --exe_path={request.ExecutablePath}");
        builder.Append($" --input_path={request.InputPath}");
        builder.Append($" --output_path={request.OutputPath}");
        builder.Append($" --error_path={Path.Combine(tempDirectory, ERROR_PATH)}");
        builder.Append($" --log_path={Path.Combine(tempDirectory, LOG_PATH)}");

        if(string.IsNullOrWhiteSpace(request.SeccompRuleName) is false)
            builder.Append($" --seccomp_rule_name={request.SeccompRuleName}");
        
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
                builder.Append($" --env=PATH={env}");
        if(request.Arguments?.Any() is true)
            foreach(var arg in request.Arguments)
                builder.Append($" --args={arg}");
        
        logger.LogInformation("Libjudger arguments: {arguments}", builder.ToString());

        return builder.ToString();
    }
}