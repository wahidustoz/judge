using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Abstractions.Models;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Exceptions;
using Ilmhub.Judge.Models;
using Microsoft.Extensions.Logging;

namespace Ilmhub.Judge;

public class LanguageService : ILanguageService
{
    private readonly int[] supportedDotnetVersions = { 6, 7 };
    private readonly ILogger<LanguageService> logger;
    private IEnumerable<ILanguageConfiguration> languageConfigurationOptions;

    public LanguageService(
        ILogger<LanguageService> logger,
        IIlmhubJudgeOptions options)
    {
        this.logger = logger;
        languageConfigurationOptions = options.LanguageConfigurations;
    }
    public ValueTask<IEnumerable<ILanguageConfiguration>> GetLangaugeConfigurationsAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting language configurations from .NET configuration options.");
        return ValueTask.FromResult(languageConfigurationOptions);
    }

    public async ValueTask<ILanguageConfiguration> GetLanguageConfigurationOrDefaultAsync(int languageId, CancellationToken cancellationToken = default)
        => (await GetLangaugeConfigurationsAsync(cancellationToken)).FirstOrDefault(x => x.Id == languageId);

    public async ValueTask<ILanguageConfiguration> GetLanguageConfigurationAsync(int languageId, CancellationToken cancellationToken = default)
    {
        var languageConfiguration = (await GetLangaugeConfigurationsAsync(cancellationToken))
            .FirstOrDefault(x => x.Id == languageId);

        if (languageConfiguration is null)
            throw new LanguageNotConfiguredException(languageId);

        return languageConfiguration;
    }

    public async ValueTask<IEnumerable<ILanguage>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        => (await GetLangaugeConfigurationsAsync(cancellationToken)).Select(x => new Language(x.Id, x.Name));

    public bool IsSupportedDotnetVersion(int languageId)
        => supportedDotnetVersions.Contains(languageId);

    public string GetDotnetProjectFileAsync(int version)
        => @$"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net{version}.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>linux-x64</RuntimeIdentifier>
  </PropertyGroup>
</Project>";

}
