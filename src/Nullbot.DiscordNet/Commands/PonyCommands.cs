using Discord;
using Discord.Commands;
using Nullbot.Derpibooru;
using System.Linq;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    public class PonyCommands : ModuleBase<SocketCommandContext>
    {
        private const int EverythingFilter = 56027;
        private const int DefaultFilter = 100073;

        [Command("derpi")]
        public async Task DerpiAsync()
        {
            var isNsfwChannel = ((ITextChannel)Context.Channel).IsNsfw;
            var imageId = await DerpibooruService.GetRandomImageAsync(
                "*",
                isNsfwChannel,
                isNsfwChannel);
            await ReplyAsync($"https://derpibooru.org/images/{imageId}");
        }

        [Command("derpi")]
        public async Task DerpiAsync(string id, params string[] args)
        {
            var suggestive = args.Contains("--suggestive");
            var nsfw = args.Contains("--nsfw");
            var isNsfwChannel = ((ITextChannel)Context.Channel).IsNsfw;

            if (nsfw && !isNsfwChannel)
            {
                await ReplyAsync("I can't do that in a non-NSFW channel.");
                return;
            }

            var imageId = await DerpibooruService.GetRandomImageAsync(
                id,
                args.Contains("--suggestive"),
                args.Contains("--nsfw"));
            await ReplyAsync($"https://derpibooru.org/images/{imageId}");
        }
    }

    [NamedArgumentType]
    public class DerpiNamedArguments
    {
        public bool Suggestive { get; set; }
        public bool Nsfw { get; set; }
    }
}
