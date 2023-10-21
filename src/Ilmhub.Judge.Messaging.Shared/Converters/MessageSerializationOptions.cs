using System.Text.Json;
using Ilmhub.Judge.Messaging.Shared.Events;
using Ilmhub.Judge.Messaging.Shared.Interfaces;

namespace Ilmhub.Judge.Messaging.Shared.Converters;

public static class MessageSerializationOptions
{
    public static JsonSerializerOptions AddJudgeMessageSerializationOptions(this JsonSerializerOptions options)
    {
        options.Converters.Insert(0, new AbstractConverter<ICompilationResult, CompilationResult>());
        options.Converters.Insert(0, new AbstractConverter<IRunResult, RunResult>());
        options.Converters.Insert(0, new AbstractConverter<ITestCaseResult, TestCaseResult>());
        options.Converters.Insert(0, new CommandConverter());
        options.Converters.Insert(0, new EventConverter());
        return options;
    }
}