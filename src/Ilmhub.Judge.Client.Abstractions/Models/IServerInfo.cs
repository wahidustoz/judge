namespace Ilmhub.Judge.Client.Abstractions.Models;

public interface IServerInfo
{
    string Hostname { get; set; }
    double Cpu { get; set; }
    long CpuCore { get; set; }
    double Memory { get; set; }
    string JudgerVersion { get; set; }
}