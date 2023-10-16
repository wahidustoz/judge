using Ilmhub.Judge.Messaging.Demo;
using Ilmhub.Judge.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddIlmhubJudge(options => 
{
    options.Settings = new()
    {
        Messaging = new()
        {
            Driver = "RabbitMQ",
            RabbitMQ = new()
            {
                Host = "localhost",
                Username = "guest",
                Password = "guest"
            }
        }
    };

    options.AddJudgeEventHandler<JudgeEventHandler>();
});
builder.Services.AddHostedService<PeriodicJudgeCommandSender>();

var app = builder.Build();

app.Run();