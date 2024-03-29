﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nullbot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _provider;
    private DiscordClient _discordClient;
    private CommandsNextExtension _commands;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration config,
        IServiceProvider provider)
    {
        _logger = logger;
        _config = config;
        _provider = provider;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var discordToken = _config.GetValue("DiscordBot:DiscordToken", string.Empty);
        var commandPrefix = _config.GetValue("DiscordBot:CommandPrefix", "nt!");
        ulong.TryParse(_config.GetValue("DiscordBot:TestGuildId", ""), out var testGuildId);

        if (string.IsNullOrEmpty(discordToken))
        {
            _logger.LogError("NullbotDiscordToken is not configured.");
        }
        else
        {
            _logger.LogInformation("Starting NullBot...");

            // Initialize client
            _discordClient = new DiscordClient(new DiscordConfiguration()
            {
                Intents = DiscordIntents.AllUnprivileged,
                Token = discordToken,
                TokenType = TokenType.Bot,
            });

            // Setup text commands
            _commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                Services = _provider,
                StringPrefixes = new[] { commandPrefix },
            });

            _commands.RegisterCommands(Assembly.GetEntryAssembly());

            // Setup slash commands
            var slashCommands = _discordClient.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = _provider,
            });

            slashCommands.RegisterCommands(Assembly.GetEntryAssembly(), testGuildId);

            // Connect to Discord
            await _discordClient.ConnectAsync();
        }

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping NullBot...");
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(60_000, stoppingToken);
        }
    }
}
