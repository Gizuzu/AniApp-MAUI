using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AniApp.Repositories.Shikimori.Models
{
    public enum ShikimoriAnimeStatus {
        [EnumMember(Value = "anons")]
        Anons,
        [EnumMember(Value = "ongoing")]
        Ongoing,
        [EnumMember(Value = "released")]
        Released
    }

    public class ShikimoriAnimeDate
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("month")]
        public int Month { get; set; }
        [JsonPropertyName("day")]
        public int Day { get; set; }
    }

    public class ShikimoriAnimeStudioInfo
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
    }

    public class ShikimoriAnimePoster
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("originalUrl")]
        public required string OriginalUrl {  get; set; }
    }

    public class ShikimoriAnimeScreenshot
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("originalUrl")]
        public required string OriginalUrl { get; set; }
    }

    public class ShikimoriAnimeInfo
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("url")]
        public required string Url { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("russian")]
        public required string NameRussian { get; set; }

        [JsonPropertyName("status")]
        public required ShikimoriAnimeStatus Status { get; set; }
        [JsonPropertyName("episodes")]
        public required int Episodes { get; set; }
        [JsonPropertyName("episodesAired")]
        public required int EpisodesAired { get; set; }
        [JsonPropertyName("airedOn")]
        public ShikimoriAnimeDate? AiredOn { get; set; }
        [JsonPropertyName("releasedOn")]
        public ShikimoriAnimeDate? ReleasedOn { get; set; }
        [JsonPropertyName("nextEpisodeAt")]
        public string? NextEpisodeAt { get; set; }

        [JsonPropertyName("poster")]
        public ShikimoriAnimePoster? Poster { get; set; }
        [JsonPropertyName("studios")]
        public required List<ShikimoriAnimeStudioInfo> studios { get; set; }
        [JsonPropertyName("screenshots")]
        public required List<ShikimoriAnimeScreenshot> screenshots { get; set; }

        [JsonPropertyName("kind")]
        public string? Kind { get; set; }
        [JsonPropertyName("rating")]
        public required string Rating { get; set; }
        [JsonPropertyName("score")]
        public required double Score { get; set; }
    }
}
