using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
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
        private readonly IServiceProvider _provider;
        private CommandService _commands;
        private DiscordSocketClient _discordClient;
        private InteractionService _interactionService;

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
                _discordClient = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    GatewayIntents = GatewayIntents.AllUnprivileged,
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1_000,
                });

                _interactionService = new InteractionService(_discordClient.Rest);

                // Setup commands
                _commands = new CommandService(new CommandServiceConfig()
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = Discord.Commands.RunMode.Async,
                });

                await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
                await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

                // Listen for messages
                _discordClient.MessageReceived += async (e) =>
                {
                    // Ignore system messages
                    if (!(e is SocketUserMessage msg)) return;

                    // Ignore bot users
                    if (msg.Source != MessageSource.User) return;

                    // Ignore self messages
                    if (msg.Author.Id == _discordClient.CurrentUser.Id) return;

                    var context = new SocketCommandContext(_discordClient, msg);

                    int argPos = 0;
                    if (msg.HasStringPrefix(commandPrefix, ref argPos) || msg.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))
                    {
                        var result = await _commands.ExecuteAsync(context, argPos, _provider);

                        if (!result.IsSuccess)
                        {
                            await context.Channel.SendMessageAsync(result.ErrorReason);
                        }
                    }
                };

                _discordClient.Ready += async () =>
                {
                    // Register Context & Slash commands
                    await _interactionService.RegisterCommandsToGuildAsync(testGuildId, true);
                };

                _discordClient.InteractionCreated += async (interaction) =>
                {
                    try
                    {
                        var context = new SocketInteractionContext(_discordClient, interaction);

                        // Execute the incoming command
                        var result = await _interactionService.ExecuteCommandAsync(context, _provider);
                    }
                    catch
                    {
                    }
                };

                // Connect to Discord
                await _discordClient.LoginAsync(TokenType.Bot, discordToken);
                await _discordClient.StartAsync();
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
