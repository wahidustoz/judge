using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Models;
using Ilmhub.Judge.Options;
using Ilmhub.Judge.Wrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge;

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
        services.AddTransient<ICompiler, DotnetCompiler>();
        services.AddTransient<ICompilationHandler, CompilationHandler>();
        services.AddTransient<IRunner, Runner>();
        services.AddTransient<ILanguageService, LanguageService>();

        return services;
    }
}
