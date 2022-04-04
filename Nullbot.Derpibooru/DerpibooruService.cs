using Flurl;
using Flurl.Http;
using Nullforce.Api.Derpibooru.JsonModels;
using System.Text.Json;

namespace Nullbot.Derpibooru;

public static class DerpibooruService
{
    private const string BaseUri = "https://derpibooru.org/api/v1/json";
    private const int EverythingFilter = 56027;
    private const int DefaultFilter = 100073;

    static DerpibooruService()
    {
        FlurlHttp.ConfigureClient(BaseUri, cli => cli
            .WithHeaders(new
            {
                Accept = "application/json",
                User_Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36",
            }));
    }

    public static async Task<int> GetRandomImageAsync(string term, bool suggestive = false, bool nsfw = false)
    {
        string searchUri = BaseUri
            .AppendPathSegment("/search/images")
            .SetQueryParam("filter_id", EverythingFilter)
            .SetQueryParam("sf", "random")
            .SetQueryParam("sd", "desc")
            .SetQueryParam("per_page", "1");

        if (nsfw)
        {
            searchUri = searchUri.SetQueryParam("q", $"!safe && !suggestive && {term}");
        }
        else if (suggestive)
        {
            searchUri = searchUri.SetQueryParam("q", $"suggestive && {term}");
        }
        else
        {
            searchUri = searchUri.SetQueryParam("q", $"safe && {term}");
        }

        var json = await searchUri.GetStringAsync();
        var searchResults = JsonSerializer.Deserialize<ImageSearchRootJson>(json);
        var images = searchResults.Images.ToList();

        return images.First().Id;
    }
}
