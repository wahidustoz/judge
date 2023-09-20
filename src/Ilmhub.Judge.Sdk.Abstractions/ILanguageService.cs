using Ilmhub.Judge.Sdk.Abstractions.Models;

namespace Ilmhub.Judge.Sdk.Abstractions;

public interface ILanguageService
{
    ValueTask<IEnumerable<ILanguageConfiguration>> GetLangaugeConfigurationsAsync(CancellationToken cancellationToken = default);
    ValueTask<ILanguageConfiguration> GetLanguageConfigurationOrDefaultAsync(int languageId, CancellationToken cancellationToken = default);
    ValueTask<ILanguageConfiguration> GetLanguageConfigurationAsync(int languageId, CancellationToken cancellationToken = default);
    ValueTask<IEnumerable<ILanguage>> GetLanguagesAsync(CancellationToken cancellationToken = default);
}
