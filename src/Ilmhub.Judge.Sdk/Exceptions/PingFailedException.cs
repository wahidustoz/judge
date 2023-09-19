using Ilmhub.Judge.Sdk.Dtos;

namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class PingFailedException : Exception
{
    public PingFailedException(PingResponse response)
        : base($"{response.Err} - {response.ErrorMessage}")
        => Repsonse = response;

    public PingResponse Repsonse { get; set; }
}
