using System.Security.Cryptography;
using System.Text;
using Ilmhub.Judge.Sdk.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ilmhub.Judge.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJudgeServerClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JudgeServerOptions>(configuration.GetSection("JudgeServer"));
        services.AddHttpClient<IJudgeServerClient, JudgeServerClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<JudgeServerOptions>>();
            var baseUri = new Uri(options.Value.BaseUrl);
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(options.Value.Token));
            var token = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            client.BaseAddress = baseUri;
            client.DefaultRequestHeaders.Add("X-Judge-Server-Token", token);
        });

        return services;
    }
}
