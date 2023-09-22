using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Exceptions;

[Serializable]
public class InvalidLanguageConfigurationException : Exception
{
    public InvalidLanguageConfigurationException(ILanguageConfiguration configuration)
        : base($"Language {configuration.Name} has invalid configuration.") 
        => LanguageConfiguration = configuration;

    public ILanguageConfiguration LanguageConfiguration { get; }
}