using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Models;

public class ServerInfo : IServerInfo
{
    public string Hostname { get; set; }
    public double Cpu { get; set; }
    public long CpuCore { get; set; }
    public double Memory { get; set; }
    public string JudgerVersion { get; set; }
}