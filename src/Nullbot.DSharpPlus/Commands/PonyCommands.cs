using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Nullbot.Derpibooru;
using System.Linq;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    public class PonyCommands : BaseCommandModule
    {
        [Command("derpi")]
        [Description("Perform image searches/retrieval on derpibooru.")]
        public async Task DerpiAsync(CommandContext context)
        {
            var imageId = await DerpibooruService.GetRandomImageAsync(
                "*",
                context.Channel.IsNSFW,
                context.Channel.IsNSFW);
            await context.RespondAsync($"https://derpibooru.org/images/{imageId}");
        }

        [Command("derpi")]
        public async Task DerpiAsync(CommandContext context, string term, params string[] args)
        {
            var suggestive = args.Contains("--suggestive");
            var nsfw = args.Contains("--nsfw");

            if (nsfw && !context.Channel.IsNSFW)
            {
                await context.RespondAsync("I can't do that in a non-NSFW channel.");
                return;
            }

            var imageId = await DerpibooruService.GetRandomImageAsync(
                term,
                suggestive,
                nsfw);
            await context.RespondAsync($"https://derpibooru.org/images/{imageId}");
        }
    }
}
