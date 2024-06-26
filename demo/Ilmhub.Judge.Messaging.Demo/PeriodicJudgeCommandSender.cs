﻿using Bogus;
using Ilmhub.Judge.Messaging.Shared.Commands;
using Ilmhub.Judge.Sdk;
using Ilmhub.Judge.Sdk.Models;

namespace Ilmhub.Judge.Messaging.Demo;

public class PeriodicJudgeCommandSender : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public PeriodicJudgeCommandSender(IServiceScopeFactory serviceScopeFactory)
        => this.serviceScopeFactory = serviceScopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IJudgeClient>();
        var testCasesId = await client.AddTestCasesAsync(
            new List<TestCase>()
            {
                new("1", "1 2", "3\n"),
                new("2", "3 4", "7\n"),
                new("3", "-1 0", "-1\n"),
            },
            stoppingToken);
        while (stoppingToken.IsCancellationRequested is false)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IJudgeCommandPublisher>();

            await service.PublishCommandAsync(JudgeCommand with { TestCaseId = testCasesId }, stoppingToken);
            // await service.PublishCommandAsync(RunCommand, stoppingToken);

            await Task.Delay(10000, stoppingToken);
        }
    }

    private static JudgeCommand JudgeCommand =>
    new()
    {
        LanguageId = 1,
        SourceCode = "#include <stdio.h>\nint main() {\n    int num1, num2;\n    scanf(\"%d %d\", &num1, &num2);\n    printf(\"%d\\n\", num1 + num2);\n    return 0;\n}",
        RequestId = Guid.NewGuid(),
        SourceId = Guid.NewGuid(),
        SourceContext = new Faker().Random.Words(3),
        Source = new Faker().Random.Words(1)
    };

    private static RunCommand RunCommand =>
    new()
    {
        LanguageId = 1,
        SourceCode = "#include <stdio.h>\nint main() {\n    int num1, num2;\n    scanf(\"%d %d\", &num1, &num2);\n    printf(\"Sum: %d\\n\", num1 + num2);\n    return 0;\n}",
        Inputs = new List<string> { "1 2", "4 5" },
        RequestId = Guid.NewGuid(),
        SourceId = Guid.NewGuid(),
        SourceContext = new Faker().Random.Words(3),
        Source = new Faker().Random.Words(1)
    };
}