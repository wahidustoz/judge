using Ilmhub.Judge.Client.Abstractions.Models;

namespace Ilmhub.Judge.Client.Models;

public class ServerInfo : IServerInfo
{
    public string Hostname { get; set; }
    public double Cpu { get; set; }
    public long CpuCore { get; set; }
    public double Memory { get; set; }
    public string JudgerVersion { get; set; }
}