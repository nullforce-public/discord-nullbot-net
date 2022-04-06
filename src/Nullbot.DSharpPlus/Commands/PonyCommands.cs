using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using Nullbot.Derpibooru;
using System.Linq;
using System.Threading.Tasks;

namespace Nullbot.Commands
{
    public class PonyCommands : BaseCommandModule
    {
        private readonly ILogger _logger;

        public PonyCommands(ILogger<PonyCommands> logger)
        {
            _logger = logger;
        }

        [Command("derpi")]
        [Description("Perform image searches/retrieval on derpibooru.")]
        public async Task DerpiAsync(CommandContext context)
        {
            _logger.LogTrace("PonyCommands: derpi called");

            var imageId = await DerpibooruService.GetRandomImageIdAsync(
                "*",
                context.Channel.IsNSFW,
                context.Channel.IsNSFW);
            await context.RespondAsync($"https://derpibooru.org/images/{imageId}");
        }

        [Command("derpi")]
        public async Task DerpiAsync(CommandContext context, int imageId)
        {
            var isNsfwChannel = context.Channel.IsNSFW;
            var imageInfo = await DerpibooruService.GetImageInfoAsync(imageId);

            if (imageInfo != null)
            {
                if (IsNsfwImage(imageInfo) && !isNsfwChannel)
                {
                    await context.RespondAsync("I can't do that in a non-NSFW channel.");
                }
                else
                {
                    await context.RespondAsync(GetImageString(imageId));
                }
            }
            else
            {
                await context.RespondAsync($"Unable to fetch image with that id ({imageId}).");
            }
        }

        [Command("derpi")]
        public async Task DerpiAsync(CommandContext context, string term, params string[] args)
        {
            var suggestive = args.Contains("--suggestive");
            var nsfw = args.Contains("--nsfw");
            var isNsfwChannel = context.Channel.IsNSFW;

            if (nsfw && !isNsfwChannel)
            {
                await context.RespondAsync("I can't do that in a non-NSFW channel.");
                return;
            }

            var imageId = await DerpibooruService.GetRandomImageIdAsync(
                term,
                suggestive,
                nsfw);
            await context.RespondAsync(GetImageString(imageId));
        }

        private static string GetImageString(int imageId) => $"https://derpibooru.org/images/{imageId}";
        private static bool IsSafeImage(Nullforce.Api.Derpibooru.JsonModels.ImageJson imageInfo) => imageInfo.Tags.Contains("safe");
        private static bool IsSuggestiveImage(Nullforce.Api.Derpibooru.JsonModels.ImageJson imageInfo) => imageInfo.Tags.Contains("suggestive");
        private static bool IsNsfwImage(Nullforce.Api.Derpibooru.JsonModels.ImageJson imageInfo) => !(IsSafeImage(imageInfo) || IsSuggestiveImage(imageInfo));
    }
}
