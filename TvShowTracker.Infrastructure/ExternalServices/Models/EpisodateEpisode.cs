using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    /// <summary>
    /// Represents an episode as returned by the Episodate external API.
    /// </summary>
    public class EpisodateEpisode
    {
        /// <summary>
        /// Gets or sets the season number of the episode.
        /// </summary>
        [JsonPropertyName("season")]
        public int Season { get; set; }

        /// <summary>
        /// Gets or sets the episode number within the season.
        /// </summary>
        [JsonPropertyName("episode")]
        public int Episode { get; set; }

        /// <summary>
        /// Gets or sets the title of the episode.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the air date of the episode, as returned by the API.
        /// </summary>
        [JsonPropertyName("air_date")]
        public string AirDate { get; set; } = string.Empty;
    }
}
