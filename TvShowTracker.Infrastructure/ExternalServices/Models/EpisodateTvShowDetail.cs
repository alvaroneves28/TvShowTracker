using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    public class EpisodateTvShowDetail
    {
        [JsonPropertyName("tvShow")]
        public EpisodateTvShowInfo TvShow { get; set; } = new();
    }
}
