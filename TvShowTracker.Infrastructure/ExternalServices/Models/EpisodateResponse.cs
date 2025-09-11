using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    public class EpisodateResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        [JsonPropertyName("tv_shows")]
        public List<EpisodateTvShow> TvShows { get; set; } = new();
    }
}
