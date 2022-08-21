using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Nullbot.Commands;

[Name("Basic")]
public class BasicCommands : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<BasicCommands> _logger;

    public BasicCommands(ILogger<BasicCommands> logger)
    {
        _logger = logger;
    }

    [Command("ping")]
    public Task PingAsync() => ReplyAsync("pong!");
}
