using AniApp.Repositories.Shikimori.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AniApp.Repositories.Shikimori
{
    public static class ShikimoriRepository
    {
        public static async Task<List<ShikimoriAnimeInfo>> search(string query, int limit = 50, int page = 1, string? mylist = null) {
            // TODO: Optimize this shit
            var parametersRaw = new Dictionary<string, dynamic?>();
            parametersRaw.Add("search", query);
            parametersRaw.Add("page", page);
            parametersRaw.Add("limit", limit);
            parametersRaw.Add("mylist", mylist);

            List<string> parametersArray = [];
            foreach (KeyValuePair<string, dynamic?> entry in parametersRaw)
            {
                if (entry.Value == null) continue;

                if (entry.Value is string)
                {
                    parametersArray.Add($"{entry.Key}: \"{entry.Value}\"");
                } else
                {
                    parametersArray.Add($"{entry.Key}: {entry.Value}");
                }
            }

            string parameters = string.Join(", ", parametersArray);

            var credentials = Globals.Credentials;
            if (credentials!.IsTokenExpired()) {
                credentials = await Auth.RefreshToken(credentials.AccessToken);
            }

            if (credentials == null)
                return [];

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "AniApp");
                client.DefaultRequestHeaders.Add("Authorization", $"{credentials.TokenType} {credentials.AccessToken}");

                var body = new
                {
                    query = "{ animes(" + parameters + ") { id name russian url studios { id name imageUrl } screenshots { id originalUrl } rating status kind rating score status episodes episodesAired poster { id originalUrl } nextEpisodeAt } }"
                };
                var response = await client.PostAsJsonAsync("https://shikimori.one/api/graphql", body);

                if (!response.IsSuccessStatusCode)
                    return [];
                
                var responseJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var animesJson = responseJson.RootElement;

                // TODO: Fix when key not exists
                if (animesJson.GetProperty("data").GetType() == null)
                    return [];

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Converters =
                        {
                            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        }
                    };

                    var animes = JsonSerializer.Deserialize<List<ShikimoriAnimeInfo>>(animesJson.GetProperty("data").GetProperty("animes").GetRawText(), options);

                    return animes ?? [];
                } catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}:\n{e.StackTrace}");
                }
            }

            return [];
        }
    }
}
