using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Exceptions;

[Serializable]
public class InvalidLanguageConfigurationException : Exception
{
    public InvalidLanguageConfigurationException(ILanguageConfiguration configuration)
        : base($"Language {configuration.Name} has invalid configuration.")
        => LanguageConfiguration = configuration;

    public ILanguageConfiguration LanguageConfiguration { get; }
}