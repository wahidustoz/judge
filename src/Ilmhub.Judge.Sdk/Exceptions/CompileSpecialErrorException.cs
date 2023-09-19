namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class CompileSpecialErrorException : Exception
{
    public CompileSpecialErrorException(string message) : base(message) { }
}
