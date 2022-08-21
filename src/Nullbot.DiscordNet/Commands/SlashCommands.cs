using Discord.Interactions;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {

        [SlashCommand("echo", "Echo an input")]
        public async Task EchoAsync(string input)
        {
            await RespondAsync(input);
        }
    }
}
