using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    /// <summary>
    /// Represents the detailed response returned by the Episodate API
    /// when querying a single TV show.
    /// </summary>
    public class EpisodateTvShowDetail
    {
        /// <summary>
        /// Gets or sets the detailed information of the TV show.
        /// </summary>
        [JsonPropertyName("tvShow")]
        public EpisodateTvShowInfo TvShow { get; set; } = new();
    }
}
