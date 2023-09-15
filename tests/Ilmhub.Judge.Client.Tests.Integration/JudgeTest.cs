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
        var cSource = @"
        int main() {
            int a, b;
            scanf(""%d %d"", &a, &b);
            printf(""%d"", a + b);
            return 0;
        }";
        var response = await client.JudgeAsync(new JudgeRequest
        {
            SourceCode = cSource,
            LanguageConfiguration = LanguageConfiguration.Defaults[ELanguageType.C],
            MaxCpuTime = 3000,
            MaxMemory = 1024 * 1024 * 128,
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = "1 2", Output = "3" },
                new TestCase { Input = "4 1", Output = "5" },
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
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
        var response = await client.JudgeAsync(new JudgeRequest
        {
            SourceCode = csSource,
            LanguageConfiguration = LanguageConfiguration.Defaults[ELanguageType.CSharp],
            MaxCpuTime = 3000,
            MaxMemory = 1024 * 1024 * 128,
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = "1 2", Output = "3" },
                new TestCase { Input = "4 1", Output = "5" },
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
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
