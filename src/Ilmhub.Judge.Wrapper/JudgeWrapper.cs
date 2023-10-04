using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Ilmhub.Judge.Wrapper.Abstractions;
using Ilmhub.Judge.Wrapper.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Wrapper;

public class JudgeWrapper : IJudgeWrapper
{
    private const string LIBJUDGER_PATH = "/usr/lib/judger/libjudger.so";
    private const string ERROR_FILENAME = "error";
    private const string LOG_FILENAME = "log";

    private readonly ILogger<JudgeWrapper> logger;
    private readonly ILinuxCommandLine cli;

    public JudgeWrapper(ILogger<JudgeWrapper> logger, ILinuxCommandLine cli)
    { 
        this.logger = logger;
        this.cli = cli;
    }

    public async ValueTask<IExecutionResult> ExecuteJudgerAsync(IExecutionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting to execute Judgerlib.so process.");
        try
        {
            var processArguments = BuildArguments(request);
            var startInfo = new ProcessStartInfo
            {
                FileName = LIBJUDGER_PATH,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = processArguments
            };
            var process = new Process { StartInfo = startInfo };

            process.Start();
            await process.WaitForExitAsync(cancellationToken);

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            logger.LogInformation(
                "Libjudger.so process finished. Exit code: {exitCode}, Output: {output}, Error: {error}.", 
                process.ExitCode,
                output,
                error);

            var resultObject = JsonSerializer.Deserialize<ExecutionResult>(output);
            resultObject.ErrorMessage = error;
            resultObject.Output = output;
            return resultObject;
        }
        catch(JsonException jsonException)
        {
            logger.LogError(jsonException, "Failed to deserialize Libjudger.so output.");
            throw;
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex, "Libjudger.so process faild while executing {executable}.", request.ExecutablePath);
            throw new JudgeProcessFailedException($"Libjudger.so process faild while executing {request.ExecutablePath}", ex);
        }
    }

    private string BuildArguments(IExecutionRequest request)
    {
        StringBuilder builder = new();
        AppendSingleArgument(builder, "exe_path", request.ExecutablePath);
        AppendSingleArgument(builder, "input_path", request.InputPath);
        AppendSingleArgument(builder, "output_path", request.OutputPath);
        AppendSingleArgument(builder, "error_path", request.ErrorPath);
        AppendSingleArgument(builder, "log_path", request.LogPath);
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

    private void AppendSingleArgument(StringBuilder builder, string key, string value) 
    {
        if(string.IsNullOrWhiteSpace(value) is false) 
            builder.Append($" --{key}={value}");
    }
}