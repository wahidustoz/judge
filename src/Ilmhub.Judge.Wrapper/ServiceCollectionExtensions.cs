using Ilmhub.Judge.Wrapper.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Ilmhub.Judge.Wrapper;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddIlmhubJudgeWrapper(this IServiceCollection services) {
        services.AddTransient<IJudgeWrapper, JudgeWrapper>();
        services.AddTransient<ILinuxCommandLine, LinuxCommandLine>();
        return services;
    }
}