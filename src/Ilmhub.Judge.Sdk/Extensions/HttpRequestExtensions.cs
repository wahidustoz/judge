namespace Ilmhub.Judge.Sdk.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsClientError(this HttpRequestException ex)
        => ex.StatusCode.HasValue
        && (int)ex.StatusCode is (>= 400 and < 500);
}