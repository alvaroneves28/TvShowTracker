using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    /// <summary>
    /// Represents detailed information about a TV show as returned by the Episodate API.
    /// Includes metadata, images, genres, ratings, and a list of episodes.
    /// </summary>
    public class EpisodateTvShowInfo
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
        /// Gets or sets the description of the TV show.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date of the TV show.
        /// </summary>
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the end date of the TV show, if applicable.
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
        /// Gets or sets the URL of the main image of the TV show.
        /// </summary>
        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the thumbnail image of the TV show.
        /// </summary>
        [JsonPropertyName("image_thumbnail_path")]
        public string ImageThumbnailPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rating of the TV show as a string (from the API).
        /// </summary>
        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of ratings the TV show has received.
        /// </summary>
        [JsonPropertyName("rating_count")]
        public string RatingCount { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the runtime of the TV show in minutes.
        /// </summary>
        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        /// <summary>
        /// Gets or sets the list of genres of the TV show.
        /// </summary>
        [JsonPropertyName("genres")]
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// Gets or sets a collection of picture URLs related to the TV show.
        /// </summary>
        [JsonPropertyName("pictures")]
        public List<string> Pictures { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of episodes for this TV show.
        /// </summary>
        [JsonPropertyName("episodes")]
        public List<EpisodateEpisode> Episodes { get; set; } = new();
    }
}
