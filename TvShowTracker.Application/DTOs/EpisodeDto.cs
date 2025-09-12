using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing a complete episode with its metadata and parent show information.
    /// Used in API responses for episode listings, search results, and detailed episode information.
    /// </summary>
    public class EpisodeDto
    {
        /// <summary>
        /// Unique system identifier for the episode record.
        /// Used for referencing the episode in API operations and database relationships.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The official title of the episode as it appears in broadcast materials.
        /// Represents the episode's unique identifier within the season context.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The season number this episode belongs to within the TV show structure.
        /// Provides organizational context for episode grouping and navigation.
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// The sequential number of this episode within its specific season.
        /// Determines the viewing order and position within the season structure.
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// The original broadcast date when the episode first aired or was released.
        /// Provides historical context and chronological reference for the content.
        /// </summary>
        public DateTime AirDate { get; set; }

        /// <summary>
        /// Brief description of the episode's plot, themes, or main events.
        /// Provides context to help users understand episode content without major spoilers.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Quality rating for the episode on a 0.0 to 10.0 scale.
        /// Represents critical assessment or audience satisfaction for this specific episode.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// The name of the TV show this episode belongs to.
        /// Provides immediate context without requiring additional API calls for show information.
        /// </summary>
        public string TvShowName { get; set; } = string.Empty;
    }
}