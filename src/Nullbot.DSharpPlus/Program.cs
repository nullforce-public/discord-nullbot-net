using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nullbot;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<Nullbot.Derpibooru.DerpibooruService>();
    })
    .Build()
    .Run();
