using System.Text.Json;
using Ilmhub.Judge.Sdk.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ilmhub.Judge.Sdk.Compiler.Tests.Integration;

public class CompileLanguagesTests
{
    private readonly IServiceProvider provider;

    public CompileLanguagesTests()
    {
        this.provider = MockProvider.SetupServiceProvider();
    }

    [Theory]
    [ClassData(typeof(LanguageTheoryData))]
    public async Task CompileLanguageAsync(string languageName, int languageId, string validSource, string invalidSource)
    {
        var compiler = provider.GetRequiredService<ICompiler>();

        Console.WriteLine($"Starting test for {languageName} with valid source.");
        var successResult = await compiler.CompileAsync(validSource, languageId, "C:\\", CancellationToken.None);
        Console.WriteLine("Success result: " + JsonSerializer.Serialize(successResult, new JsonSerializerOptions() { WriteIndented = true }));
        Assert.True(successResult.IsSuccess);

        Console.WriteLine($"Starting test for {languageName} with invalid source.");
        var failingResult = await compiler.CompileAsync(invalidSource, languageId, "C:\\", CancellationToken.None);
        Console.WriteLine("Failing result: " + JsonSerializer.Serialize(failingResult, new JsonSerializerOptions() { WriteIndented = true }));
        Assert.True(failingResult.IsSuccess is false);
    }
}



public class LanguageTheoryData : TheoryData<string, int, string, string>
{
    public LanguageTheoryData()
    {
        // Add language information to the TheoryData
        Add("C", 1,
            @"
                #include <stdio.h>
                int main() {
                    int num1, num2;
                    scanf(""%d %d"", &num1, &num2);
                    printf(""Sum: %d\n"", num1 + num2);
                    return 0;
                }",
            @"
                #include <stdio.h>
                int main() {
                    int num1, num2;
                    scanf(""%d %d"", &num1, &num2);
                    printf(""Sum: %d\n"", num1 + num2)          // Syntax Error: Missing semicolon
                    return 0; 
                }");

        Add("C++", 2,
            @"
                #include <iostream>
                using namespace std;
                int main() {
                    int num1, num2;
                    cin >> num1 >> num2;
                    cout << ""Sum: "" << num1 + num2 << endl;
                    return 0;
                }",
            @"
                #include <iostream>
                using namespace std;
                int main() {
                    int num1, num2;
                    cin >> num1 >> num2;
                    cout << ""Sum: "" << num1 + num2 << end; // Syntax Error: Should be 'endl;'
                    return 0;
                }");

        Add("C#", 3,
            @"
                using System;
                class Program {
                    static void Main() {
                        int num1, num2;
                        string input = Console.ReadLine();
                        string[] numbers = input.Split();
                        num1 = int.Parse(numbers[0]);
                        num2 = int.Parse(numbers[1]);
                        Console.WriteLine(""Sum: "" + (num1 + num2));
                    }
                }",
            @"
                using System;
                class Program {
                    static void Main() {
                        int num1, num2;
                        string input = Console.ReadLine();
                        string[] numbers = input.Split()            // missing ;
                        num1 = int.Parse(numbers[0]);
                        num2 = int.Parse(numbers[1]);
                        Console.WriteLine(""Sum: "" + (num1 + num2));
                    }
                }");

        Add("Python3", 4,
            @"
num1, num2 = map(int, input().split())
print('Sum:', num1 + num2)",
            @"
num1, num2 = map(int input().split())
print('Sum:', num1 + num2) # Syntax Error: Missing dot comma after 'int'
            }");

        Add("Go", 5,
            @"
                package main

                import (
                    ""fmt""
                )

                func main() {
                    var num1, num2 int
                    fmt.Scan(&num1, &num2)
                    fmt.Println(""Sum: "", num1 + num2)
                }",
            @"
                package main

                import (
                    ""fmt""
                )

                func main() {
                    var num1, num2 int
                    fmt.Scan(&num1, &num2)
                    fmt.Println(""Sum: "", num1 // Syntax Error: Missing '+' after 'num1'
                }");

        Add("Java", 6,
            @"
                import java.util.Scanner;

                public class Main {
                    public static void main(String[] args) {
                        Scanner scanner = new Scanner(System.in);
                        int num1 = scanner.nextInt();
                        int num2 = scanner.nextInt();
                        System.out.println(""Sum: "" + (num1 + num2));
                    }
                }",
            @"
                import java.util.Scanner;

                public class Main {
                    public static void main(String[] args) {
                        Scanner scanner = new Scanner(System.in);
                        int num1 = scanner.nextInt();
                        int num2 = scanner.nextInt()            // missing semi colon
                        System.out.println(""Sum: "" + (num1 + num2));
                    }
                }");
    }
}
