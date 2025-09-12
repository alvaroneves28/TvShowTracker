using System.Text.Json.Serialization;

namespace TvShowTracker.Infrastructure.ExternalServices.Models
{
    /// <summary>
    /// Represents the response returned by the Episodate API when querying TV shows.
    /// Includes pagination information and a list of TV shows.
    /// </summary>
    public class EpisodateResponse
    {
        /// <summary>
        /// Gets or sets the total number of TV shows available in the API response.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the current page number in the paginated response.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages available in the response.
        /// </summary>
        [JsonPropertyName("pages")]
        public int Pages { get; set; }

        /// <summary>
        /// Gets or sets the collection of TV shows returned by the API.
        /// </summary>
        [JsonPropertyName("tv_shows")]
        public List<EpisodateTvShow> TvShows { get; set; } = new();
    }
}
