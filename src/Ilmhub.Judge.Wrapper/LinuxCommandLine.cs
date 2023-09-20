using System.Diagnostics;
using System.Text.RegularExpressions;
using Ilmhub.Judge.Wrapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Wrapper;

public class LinuxCommandLine : ILinuxCommandLine
{
    private readonly ILogger<LinuxCommandLine> logger;

    public LinuxCommandLine(ILogger<LinuxCommandLine> logger) => this.logger = logger;

    public ValueTask ChangePathModeAsync(string mode, string path, CancellationToken cancellationToken = default)
        => RunCommandAsync("chmod", $"{mode} {path}");

    public ValueTask AddPathOwnerAsync(string owner, string path, CancellationToken cancellationToken = default)
        => RunCommandAsync("chown", $"{owner} {path}");

    public async ValueTask RunCommandAsync(string command, string arguments, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting to run linux command: {command} {arguments}", command, arguments);
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        await process.WaitForExitAsync(cancellationToken);

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if(process.ExitCode is not 0 || string.IsNullOrWhiteSpace(error) is false)
            throw new LinuxCommandFailedException(command, arguments, output, error);

        logger.LogInformation("Finished running linux command: {command} {arguments}, output: {output}, error: {error}", command, arguments, output, error);
    }

    public ValueTask RemoveFolderAsync(string path, CancellationToken cancellationToken = default)
        => RunCommandAsync("rm", $"-rf {path}");

    public IEnumerable<string> SplitCommand(string command)
    {
        List<string> arguments = new List<string>();
        Regex regex = new Regex(@"[\""]([^\""]+)\""|([^ ]+)");

        foreach (Match match in regex.Matches(command))
        {
            string argument = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            arguments.Add(argument);
        }

        return arguments;
    }
}
