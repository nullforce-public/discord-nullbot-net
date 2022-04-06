   using Flurl;
using Flurl.Http;
using Nullforce.Api.Derpibooru.JsonModels;
using System.Text.Json;

namespace Nullbot.Derpibooru;

public class DerpibooruService
{
    private const string BaseUri = "https://derpibooru.org/api/v1/json";
    private const int EverythingFilter = 56027;
    private const int DefaultFilter = 100073;

    public DerpibooruService()
    {
        FlurlHttp.ConfigureClient(BaseUri, cli => cli
            .WithHeaders(new
            {
                Accept = "application/json",
                User_Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36",
            }));
    }

    public async Task<ImageJson?> GetImageInfoAsync(int id)
    {
        string imageUri = BaseUri
            .AppendPathSegment($"/images/{id}");

        var imageInfo = await imageUri.GetJsonAsync<ImageRootJson>();
        return imageInfo?.Image;
    }

    public async Task<int> GetRandomImageIdAsync(string term, bool suggestive = false, bool nsfw = false)
    {
        string searchUri = BaseUri
            .AppendPathSegment("/search/images")
            .SetQueryParam("filter_id", EverythingFilter)
            .SetQueryParam("sf", "random")
            .SetQueryParam("sd", "desc")
            .SetQueryParam("per_page", "1");

        if (nsfw)
        {
            searchUri = searchUri.SetQueryParam("q", $"!safe && !suggestive && first_seen_at.gt:10 days ago && score.gt:50 && {term}");
        }
        else if (suggestive)
        {
            searchUri = searchUri.SetQueryParam("q", $"suggestive && first_seen_at.gt:10 days ago && score.gt:50 && {term}");
        }
        else
        {
            searchUri = searchUri.SetQueryParam("q", $"safe && first_seen_at.gt:10 days ago && score.gt:50 && {term}");
        }

        var json = await searchUri.GetStringAsync();
        var searchResults = JsonSerializer.Deserialize<ImageSearchRootJson>(json);

        return searchResults?.Images.FirstOrDefault()?.Id ?? -1;
    }
}
