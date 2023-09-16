namespace Ilmhub.Judge.Client.Exceptions;

[Serializable]
public class JudgeFailedException : Exception
{
    public JudgeFailedException(string message) : base(message) { }
}