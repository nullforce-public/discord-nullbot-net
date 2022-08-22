using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Nullbot.Commands;

//[Group("Basic")]
public class BasicCommands : BaseCommandModule
{
    private readonly ILogger<BasicCommands> _logger;

    public BasicCommands(ILogger<BasicCommands> logger)
    {
        _logger = logger;
    }

    [Command("ping")]
    [Description("Responds with a pong message.")]
    public async Task PingAsync(CommandContext context) => await context.RespondAsync("pong!");
}
