namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class JudgeClientRequestValidationException : Exception {
    public JudgeClientRequestValidationException(HttpRequestException innerException)
        : base("Request failed due to validation errors.", innerException) { }
}