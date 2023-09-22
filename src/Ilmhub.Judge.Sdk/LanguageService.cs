using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Abstractions.Models;
using Ilmhub.Judge.Sdk.Exceptions;
using Ilmhub.Judge.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge.Sdk;

public class LanguageService : ILanguageService
{
    private readonly ILogger<LanguageService> logger;
    private IEnumerable<ILanguageConfiguration> languageConfigurationOptions;

    public LanguageService(
        ILogger<LanguageService> logger,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        this.languageConfigurationOptions = options.LanguageConfigurations;
    }
    public ValueTask<IEnumerable<ILanguageConfiguration>> GetLangaugeConfigurationsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting language configurations from .NET configuration options.");
        return ValueTask.FromResult<IEnumerable<ILanguageConfiguration>>(languageConfigurationOptions);
    }

    public async ValueTask<ILanguageConfiguration> GetLanguageConfigurationOrDefaultAsync(int languageId, CancellationToken cancellationToken = default)
        => (await GetLangaugeConfigurationsAsync(cancellationToken)).FirstOrDefault(x => x.Id == languageId);
    
    public async ValueTask<ILanguageConfiguration> GetLanguageConfigurationAsync(int languageId, CancellationToken cancellationToken = default)
    {
        var languageConfiguration = (await GetLangaugeConfigurationsAsync(cancellationToken))
            .FirstOrDefault(x => x.Id == languageId);
        
        if(languageConfiguration is null)
            throw new LanguageNotConfiguredException(languageId);

        return languageConfiguration;
    }

    public async ValueTask<IEnumerable<ILanguage>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        => (await GetLangaugeConfigurationsAsync(cancellationToken)).Select(x => new Language(x.Id, x.Name));
}
