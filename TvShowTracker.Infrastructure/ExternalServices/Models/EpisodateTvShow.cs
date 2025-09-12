using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    /// <summary>
    /// Represents a TV show as returned by the Episodate external API.
    /// Contains metadata such as dates, country, network, and status.
    /// </summary>
    public class EpisodateTvShow
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TV show in the Episodate API.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the TV show.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the permalink (slug) used for the show's URL on Episodate.
        /// </summary>
        [JsonPropertyName("permalink")]
        public string Permalink { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the TV show started airing (as returned by the API).
        /// </summary>
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the TV show ended (if applicable).
        /// </summary>
        [JsonPropertyName("end_date")]
        public string? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the country of origin of the TV show.
        /// </summary>
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the network or channel where the TV show was broadcast.
        /// </summary>
        [JsonPropertyName("network")]
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the TV show (e.g., Running, Ended).
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the show's thumbnail image.
        /// </summary>
        [JsonPropertyName("image_thumbnail_path")]
        public string ImageThumbnailPath { get; set; } = string.Empty;
    }
}
