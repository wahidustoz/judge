using System.Configuration.Assemblies;
using Ilmhub.Judge.Client.Abstractions;
using Ilmhub.Judge.Client.Abstractions.Models;
using Ilmhub.Judge.Client.Exceptions;
using Ilmhub.Judge.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ilmhub.Judge.Client.Tests.Integration;

public partial class JudgeTest
{
    [Fact]
    public async Task CSharpPositiveTest_CorrectSumCalculation()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class SumCalculator
    {    
        public static void Main(string[] args)
        {
            var numbers = Console.ReadLine().Split(' ');
            int sum = 0;
            foreach (var num in numbers)
            {
                sum += int.Parse(num);
            }
            Console.WriteLine(sum);
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
                new TestCase { Input = "1 2 3", Output = "6" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("6", testCaseResult.Output.Trim());
    }

    [Fact]
    public async Task CSharpPositiveTest_HandlingLargeInput()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class LargeInputHandler
    {    
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            Console.WriteLine(""Input received: "" + input);
        }
    }";
        var largeInput = new string('A', 1024 * 1024); // Simulate a large input
        var response = await client.JudgeAsync(new JudgeRequest
        {
            SourceCode = csSource,
            LanguageConfiguration = LanguageConfiguration.Defaults[ELanguageType.CSharp],
            MaxCpuTime = 3000,
            MaxMemory = 1024 * 1024 * 128,
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = largeInput, Output = "Input received: " + largeInput }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("Input received: " + largeInput, testCaseResult.Output.Trim());
    }

    [Fact]
    public async Task CSharpPositiveTest_HandleSpecialCharacters()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class SpecialCharacterHandler
    {    
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            Console.WriteLine(""Special characters: "" + input);
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
                new TestCase { Input = "!@#$%^&*()", Output = "Special characters: !@#$%^&*()" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("Special characters: !@#$%^&*()", testCaseResult.Output.Trim());
    }

    [Fact]
    public async Task CSharpPositiveTest_HandleInputWithWhitespaces()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class InputWithWhitespaces
    {    
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            Console.WriteLine(""User input: "" + input);
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
                new TestCase { Input = "Hello\tWorld\n", Output = "User input: Hello\tWorld\n" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("User input: Hello\tWorld\n", testCaseResult.Output);
    }

    [Fact]
    public async Task CSharpPositiveTest_HandleDecimalNumbers()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class DecimalNumberHandler
    {    
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            double number = double.Parse(input);
            Console.WriteLine(""Squared: "" + (number * number));
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
                new TestCase { Input = "3.14", Output = "Squared: 9.8596" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("Squared: 9.8596", testCaseResult.Output.Trim());
    }

    [Fact]
    public async Task CSharpNegativeTest_InvalidInput()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class InputValidator
    {    
        public static void Main(string[] args)
        {
            var input = Console.ReadLine();
            int number;
            if (int.TryParse(input, out number))
            {
                Console.WriteLine(""Valid: "" + number);
            }
            else
            {
                Console.WriteLine(""Invalid input"");
            }
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
                new TestCase { Input = "abc", Output = "Invalid input" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.Success, testCaseResult.Status);
        Assert.Equal("Invalid input", testCaseResult.Output.Trim());
    }

    [Fact]
    public async Task CSharpNegativeTest_RuntimeError()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class DivisionByZero
    {    
        public static void Main(string[] args)
        {
            int a = 5;
            int b = 0;
            Console.WriteLine(a / b); // Division by zero
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
                new TestCase { Input = "", Output = "" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.RuntimeError, testCaseResult.Status);
    }

    [Fact]
    public async Task CSharpNegativeTest_MemoryLimitExceeded()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class MemoryEater
    {    
        public static void Main(string[] args)
        {
            var arr = new int[1024 * 1024 * 256]; // Allocate a large array
            for(int i = 0; i < 256; i++);
        }
    }";
        var response = await client.JudgeAsync(new JudgeRequest
        {
            SourceCode = csSource,
            LanguageConfiguration = LanguageConfiguration.Defaults[ELanguageType.CSharp],
            MaxCpuTime = 3000,
            MaxMemory = 64, // Set a lower memory limit
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = "", Output = "" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.MemoryLimitExceeded, testCaseResult.Status);
    }

    [Fact]
    public async Task CSharpNegativeTest_RealTimeLimitExceeded()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class RealTimeConsumer
    {    
        public static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(5000); // Sleep for 5 seconds
            Console.WriteLine(""Done"");
        }
    }";
        var languageConfigWithLowRealTimeMemory = LanguageConfiguration.Defaults[ELanguageType.CSharp];
        languageConfigWithLowRealTimeMemory.Compile.MaxRealTime = 100;
        var response = await client.JudgeAsync(new JudgeRequest
        {
            SourceCode = csSource,
            LanguageConfiguration = languageConfigWithLowRealTimeMemory,
            MaxCpuTime = 3000,
            MaxMemory = 1024 * 1024 * 128,
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = "", Output = "Done" }
            },
            ShouldReturnOutput = true
        });

        Assert.True(response.IsSuccess);
        Assert.NotEmpty(response.TestCases);
        var testCaseResult = response.TestCases.First();
        Assert.Equal(EJudgeStatus.RealTimeLimitExceeded, testCaseResult.Status);
    }

    [Fact]
    public async Task CSharpNegativeTest_CompilationError()
    {
        var client = provider.GetRequiredService<IJudgeServerClient>();
        var csSource = @"
    using System;
    public class SyntaxError
    {    
        public static void Main(string[] args)
        {
            // Missing semicolon at the end of the line
            Console.WriteLine(""Hello, world"")
        }
    }";
        var judgeTask = client.JudgeAsync(new JudgeRequest
        {
            SourceCode = csSource,
            LanguageConfiguration = LanguageConfiguration.Defaults[ELanguageType.CSharp],
            MaxCpuTime = 3000,
            MaxMemory = 1024 * 1024 * 128,
            TestCases = new List<ITestCase>
            {
                new TestCase { Input = "", Output = "" }
            },
            ShouldReturnOutput = true
        }).AsTask();

        await Assert.ThrowsAsync<JudgeFailedException>(() => judgeTask);
        var exception = judgeTask.Exception.InnerExceptions.First(ex => ex is JudgeFailedException);
        Assert.Contains("CompileError", exception.Message);
    }

}