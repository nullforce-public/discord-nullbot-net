using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    //[Group("Basic")]
    public class BasicCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Responds with a pong message.")]
        public async Task PingAsync(CommandContext context)
        {
            await context.RespondAsync("pong!");
        }
    }
}
