using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Nullbot.Derpibooru;
using System.Linq;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    public class PonyCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly DerpibooruService _derpibooru;

        public PonyCommands(
            ILogger<PonyCommands> logger,
            DerpibooruService derpibooru)
        {
            _logger = logger;
            _derpibooru = derpibooru;
        }

        [Command("derpi")]
        public async Task DerpiAsync()
        {
            _logger.LogTrace("PonyCommands: derpi called");

            var isNsfwChannel = ((ITextChannel)Context.Channel).IsNsfw;
            var imageId = await _derpibooru.GetRandomImageIdAsync(
                "*",
                isNsfwChannel,
                isNsfwChannel);
            await ReplyAsync($"https://derpibooru.org/images/{imageId}");
        }

        [Command("derpi")]
        public async Task DerpiAsync(int imageId)
        {
            var isNsfwChannel = ((ITextChannel)Context.Channel).IsNsfw;
            var imageInfo = await _derpibooru.GetImageInfoAsync(imageId);

            if (imageInfo != null)
            {
                if (IsNsfwImage(imageInfo) && !isNsfwChannel)
                {
                    await ReplyAsync("I can't do that in a non-NSFW channel.");
                }
                else
                {
                    await ReplyAsync(GetImageString(imageId));
                }
            }
            else
            {
                await ReplyAsync($"Unable to fetch image with that id ({imageId}).");
            }
        }

        [Command("derpi")]
        public async Task DerpiAsync(string term, params string[] args)
        {
            var suggestive = args.Contains("--suggestive");
            var nsfw = args.Contains("--nsfw");
            var isNsfwChannel = ((ITextChannel)Context.Channel).IsNsfw;

            if (nsfw && !isNsfwChannel)
            {
                await ReplyAsync("I can't do that in a non-NSFW channel.");
                return;
            }

            var imageId = await _derpibooru.GetRandomImageIdAsync(
                term,
                args.Contains("--suggestive"),
                args.Contains("--nsfw"));
            await ReplyAsync(GetImageString(imageId));
        }

        private static string GetImageString(int imageId) => $"https://derpibooru.org/images/{imageId}";
        private static bool IsSafeImage(Nullforce.Api.JsonModels.Philomena.ImageJson imageInfo) => imageInfo.Tags.Contains("safe");
        private static bool IsSuggestiveImage(Nullforce.Api.JsonModels.Philomena.ImageJson imageInfo) => imageInfo.Tags.Contains("suggestive");
        private static bool IsNsfwImage(Nullforce.Api.JsonModels.Philomena.ImageJson imageInfo) => !(IsSafeImage(imageInfo) || IsSuggestiveImage(imageInfo));
    }

    [NamedArgumentType]
    public class DerpiNamedArguments
    {
        public bool Suggestive { get; set; }
        public bool Nsfw { get; set; }
    }
}
