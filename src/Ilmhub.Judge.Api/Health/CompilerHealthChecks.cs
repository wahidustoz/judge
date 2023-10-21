using System.Collections.ObjectModel;
using Ilmhub.Judge.Wrapper.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ilmhub.Judge.Api;

public class CompilerHealthChecks : IHealthCheck
{
    private readonly ILinuxCommandLine cli;
    public CompilerHealthChecks(ILinuxCommandLine cli) => this.cli = cli;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var compilerOutputs = new Dictionary<string, (bool IsSuccess, string Output, string ErrorMessage)>
        {
            { "GCC", await cli.TryRunAsync("/usr/bin/gcc", "--version", cancellationToken) },
            { "G++", await cli.TryRunAsync("/usr/bin/g++", "--version", cancellationToken) },
            { "C# Mono", await cli.TryRunAsync("/usr/bin/mcs", "--version", cancellationToken) },
            { "Python 3", await cli.TryRunAsync("/usr/bin/python3", "--version", cancellationToken) },
            { ".NET", await cli.TryRunAsync("/usr/bin/dotnet", "--list-sdks", cancellationToken) },
            { "GO", await cli.TryRunAsync("/usr/bin/go", "version", cancellationToken) },
        };

        var data = new ReadOnlyDictionary<string, object>(compilerOutputs.ToDictionary(x => x.Key, x =>
            (object)new
            {
                x.Value.IsSuccess,
                x.Value.Output,
                x.Value.ErrorMessage
            }));

        if (compilerOutputs.All(x => x.Value.IsSuccess == false))
            return new HealthCheckResult(HealthStatus.Unhealthy, data: data);
        else if (compilerOutputs.All(x => x.Value.IsSuccess == true))
            return new HealthCheckResult(HealthStatus.Healthy, data: data);
        else
            return new HealthCheckResult(HealthStatus.Degraded, data: data);
    }
}