using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace Nullbot.Commands;

public class SlashCommands : ApplicationCommandModule
{
    [SlashCommand("echo", "Echo an input")]
    public async Task EchoAsync(
        InteractionContext context,
        [Option("input", "input")] string input)
    {
        await context.CreateResponseAsync(input);
    }
}
