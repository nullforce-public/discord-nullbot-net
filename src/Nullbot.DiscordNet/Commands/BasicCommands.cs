using Discord.Commands;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    [Name("Basic")]
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public Task PingAsync() => ReplyAsync("pong!");
    }
}
