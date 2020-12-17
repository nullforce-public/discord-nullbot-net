using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nullbot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private DiscordClient _discordClient;

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

                // Listen for messages
                _discordClient.MessageCreated += async (e) =>
                {
                    if (e.Message.Content.ToLower().StartsWith("ping"))
                    {
                        await e.Message.RespondAsync("pong!");
                    }
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
