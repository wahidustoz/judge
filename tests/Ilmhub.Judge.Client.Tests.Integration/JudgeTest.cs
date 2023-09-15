using Ilmhub.Judge.Client.Abstractions;
using Ilmhub.Judge.Client.Abstractions.Models;
using Ilmhub.Judge.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Client.Tests.Integration;

public class JudgeTest
{
    private readonly ServiceProvider provider;

    public JudgeTest()
        => provider = SetupServiceProvider("http://localhost:12358", "123token");

    [Fact]
    public async Task CTestSuccessfullyJudges()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
        int main() {
            int a, b;
            scanf(""%d %d"", &a, &b);
            printf(""%d"", a + b);
            return 0;
        }";
        var response = await client.JudgeAsync(
            sourceCode: csSource,
            languageConfiguration: LanguageConfiguration.Defaults[ELanguageType.C],
            maxCpuTime: 3000,
            maxMemory: 1024 * 1024 * 128,
            testCases: new List<ITestCase>
            {
                new TestCase { Input = "1 2", Output = "3" },
                new TestCase { Input = "4 1", Output = "5" },
            },
            showsOutput: true
        );

        Assert.Contains(@"""err"": null", response);
    }

    [Fact]
    public async Task CSharpTestSuccessfullyJudges()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
        using System;
        using System.Linq;
        public class HelloWorld
        {    
            public static void Main(string[] args)
            {
                var numbers = Console.ReadLine()
                    .Split(' ')
                    .Select(int.Parse)
                    .ToArray(); 
                Console.WriteLine($""{numbers[0]+numbers[1]}"");
            }
        }";
        var response = await client.JudgeAsync(
            sourceCode: csSource,
            languageConfiguration: LanguageConfiguration.Defaults[ELanguageType.CSharp],
            maxCpuTime: 3000,
            maxMemory: 1024 * 1024 * 128,
            testCases: new List<ITestCase>
            {
                new TestCase { Input = "1 2", Output = "3" },
                new TestCase { Input = "4 1", Output = "5" },
            }
        );

        Assert.Contains(@"""err"": null", response);
    }

    private ServiceProvider SetupServiceProvider(string baseUrl, string token)
    {
        var configuration = ConfigurationMockFactory.Create(new Dictionary<string, string>
            {
                { "JudgeServer:BaseUrl", baseUrl },
                { "JudgeServer:Token", token }
            });

        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddJudgeServerClient(configuration);

        return services.BuildServiceProvider();
    }
}
