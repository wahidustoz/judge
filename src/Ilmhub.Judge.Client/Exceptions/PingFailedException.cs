using Ilmhub.Judge.Client.Dtos;

namespace Ilmhub.Judge.Client.Exceptions;

[Serializable]
public class PingFailedException : Exception
{
    public PingFailedException(PingResponse response) 
        : base($"{response.Err} - {response.ErrorMessage}") 
        => Repsonse = response;

    public PingResponse Repsonse { get; set; }
}
