using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Nullbot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private DiscordClient _discordClient;
        private CommandsNextModule _commands;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var discordToken = _config.GetValue("DiscordBot:DiscordToken", string.Empty);
            var commandPrefix = _config.GetValue("DiscordBot:CommandPrefix", "nt!");

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
                    Token = discordToken,
                    TokenType = TokenType.Bot,
                });

                // Setup commands
                _commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration()
                {
                    StringPrefix = commandPrefix,
                });

                _commands.RegisterCommands(Assembly.GetEntryAssembly());

                // Listen for messages
                _discordClient.MessageCreated += async (e) =>
                {
                };

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
}
