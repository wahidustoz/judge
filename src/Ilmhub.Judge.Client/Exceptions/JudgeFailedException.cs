using Ilmhub.Judge.Client.Dtos;

namespace Ilmhub.Judge.Client.Exceptions;

public class JudgeFailedException : Exception
{
    public JudgeFailedException(JudgeResponse response) 
    : base($"Judge failed: {response.ErrorMessage}") 
        => Response = response;

    public JudgeResponse Response { get; set; }
}