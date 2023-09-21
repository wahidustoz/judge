using Ilmhub.Judge.Sdk.Abstractions;
using Ilmhub.Judge.Sdk.Models;
using Ilmhub.Judge.Sdk.Options;
using Ilmhub.Judge.Wrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIlmhubJudge(this IServiceCollection services, IConfigurationSection judgeSection)
    {
        services.AddIlmhubJudge(options =>
        {
            var languages = new List<LanguageConfiguration>();
            judgeSection.Bind(options);
            judgeSection.Bind("LanguageConfigurations", languages);

            options.LanguageConfigurations = languages;
        });
        return services;
    }

    public static IServiceCollection AddIlmhubJudge(this IServiceCollection services, Action<IIlmhubJudgeOptions> configure)
    {
        var options = new IlmhubJudgeOptions();
        configure(options);
        // TODO: implement fluent validation for options here
        services.AddSingleton<IIlmhubJudgeOptions>(options);

        services.AddIlmhubJudgeWrapper();
        services.AddTransient<IJudger, Judger>();
        services.AddTransient<ICompiler, Compiler>();
        services.AddTransient<IRunner, Runner>();
        services.AddTransient<ILanguageService, LanguageService>();

        return services;
    }
}
