namespace Ilmhub.Judge.Wrapper.Exceptions;

public class LinuxCommandFailedException : Exception {
    public LinuxCommandFailedException(string command, string arguments, string output, string error)
        : base($"Linux command failed. Command: {command}, Arguments: {arguments}, Output: {output}, Error: {error}") { }
}