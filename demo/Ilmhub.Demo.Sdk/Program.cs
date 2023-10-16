using System.Text.Json;
using Ilmhub.Demo.Sdk;
using Ilmhub.Judge;
using Ilmhub.Judge.Abstractions;
using Ilmhub.Judge.Abstractions.Options;
using Ilmhub.Judge.Wrapper.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(configure => 
{
    configure.AddFilter("Ilmhub.Judge.*", LogLevel.Trace);
});
builder.Services.AddIlmhubJudge(Setup.ConfigureLanguages);

var app = builder.Build();
var compiler = app.Services.GetRequiredService<ICompilationHandler>();
var runner = app.Services.GetRequiredService<IRunner>();
var judger = app.Services.GetRequiredService<IJudger>();
var cli = app.Services.GetRequiredService<ILinuxCommandLine>();
var languageService = app.Services.GetRequiredService<ILanguageService>();
var users = app.Services.GetRequiredService<IIlmhubJudgeOptions>().SystemUsers;


var cc = await compiler.CompileAsync(
        source: @"Console.WriteLine(""Hello world"");", 
        languageId: 6);
Console.WriteLine(JsonSerializer.Serialize(cc, new JsonSerializerOptions() { WriteIndented = true }));


// return;

// var cSource = @"
// #include <stdio.h>
// int main() {
//     int num1, num2;
//     scanf(""%d %d"", &num1, &num2);
//     printf(""Sum: %d\n"", num1 + num2);
//     return 0;
// }";
// // var pythonSource = @"
// // num1, num2 = map(int, input().split())
// // print(num1 + num2)";
// // var pythonSource = @"
// // print('22222')
// // print('2   2')
// // print('2   2')
// // print('22222')
// // print()
// // ";

// var testCases = new List<TestCase>
// {
//     new TestCase { Id = 1, Input = "1 2", Output = "3" },
//     new TestCase { Id = 2, Input = "4 5", Output = "9" },
// };

// var judgeResult = await judger.JudgeAsync(
//     languageId: 1,
//     source: cSource,
//     maxCpu: 3000,
//     maxMemory: 128 * 1024 * 1024,
//     // testCasesFolder: "/judger/testcases2",
//     testCases: testCases);

// Console.WriteLine(JsonSerializer.Serialize(judgeResult, new JsonSerializerOptions() { WriteIndented = true }));

// return;


// var languages = new Dictionary<string, (int Id, string Source)>
// {
//     { "C Warning", (1, @"int main() { printf(""Hello, World!""); return 0; }") },
//     { "C ++", (2, "#include <iostream>\n\nint main() { std::cout << \"Hello, world!\" << std::endl; return 0; }") },
//     { "Mono C#", (3, "using System; class Program { static void Main() { Console.WriteLine(\"Hello, World!\"); } }" ) },
//     { "Python 3", (4, @"print(""Hello, World!"")" ) },
//     { "Go", (5, @"package main; import ""fmt""; func main() { fmt.Println(""Hello, World!"") }" ) },
// };


// foreach(var language in languages)
// {
//     Console.WriteLine($"Compiling {language.Key}");
//     var folder = Directory.CreateDirectory(Path.Combine("/judger", Guid.NewGuid().ToString())).FullName;
//     var compilationResult = await compiler.CompileAsync(
//         source: language.Value.Source, 
//         languageId: language.Value.Id,
//         environmentFolder: folder);
//     Console.WriteLine(JsonSerializer.Serialize(compilationResult, new JsonSerializerOptions() { WriteIndented = true }));

//     if(compilationResult.IsSuccess)
//     {
//         Console.WriteLine($"Running {language.Key}");
//         var runResult = await runner.RunAsync(
//             languageId: language.Value.Id,
//             input: string.Empty,
//             executableFilename: compilationResult.ExecutableFilePath,
//             maxCpu: 3000,
//             maxMemory: -1,
//             environmentFolder: folder);
//         Console.WriteLine(JsonSerializer.Serialize(runResult, new JsonSerializerOptions() { WriteIndented = true }));

//         if(runResult.IsSuccess)
//         {
//             Console.WriteLine("Compile and run is success. Cleaning temp folder");
//             await cli.RunCommandAsync("rm", $"-rf {folder}");
//         }
//     }

// }

// // string goSource = ;
// // await Compile(goSource, 5);

// // string javaSource = "public class Main { public static void main(String[] args) { System.out.println(\"Hello, World!\"); } }";
// // await Compile(javaSource, languageId: 6);