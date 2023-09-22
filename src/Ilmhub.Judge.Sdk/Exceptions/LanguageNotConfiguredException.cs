namespace Ilmhub.Judge.Sdk.Exceptions;

public class LanguageNotConfiguredException : Exception
{
    public LanguageNotConfiguredException(int languageId)
        : base($"Language with id {languageId} not confired") { }
}