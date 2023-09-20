namespace Ilmhub.Judge.Wrapper.Abstractions;

public interface ILinuxCommandLine
{
    ValueTask RunCommandAsync(string command, string arguments, CancellationToken cancellationToken = default);
    ValueTask AddPathOwnerAsync(string owner, string path, bool recursive = false, CancellationToken cancellationToken = default);
    ValueTask ChangePathModeAsync(string mode, string path, bool recursive = false, CancellationToken cancellationToken = default);
    ValueTask RemoveFolderAsync(string path, CancellationToken cancellationToken = default);
    IEnumerable<string> SplitCommand(string command);
}
