using Ilmhub.Judge.Abstractions.Models;

namespace Ilmhub.Judge.Abstractions;

public interface ILanguageService
{
    ValueTask<IEnumerable<ILanguageConfiguration>> GetLangaugeConfigurationsAsync(CancellationToken cancellationToken = default);
    ValueTask<ILanguageConfiguration> GetLanguageConfigurationOrDefaultAsync(int languageId, CancellationToken cancellationToken = default);
    ValueTask<ILanguageConfiguration> GetLanguageConfigurationAsync(int languageId, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<ILanguage>> GetLanguagesAsync(CancellationToken cancellationToken = default);
    bool IsSupportedDotnetVersion(int languageId);
    string GetDotnetProjectFileAsync(int version);
}