namespace TvShowTracker.Core.Entities
{
    /// <summary>
    /// Domain entity representing an individual episode within a TV show's episode catalog.
    /// Encapsulates episode metadata, organizational structure, and content information for comprehensive show management.
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Unique identifier for the episode entity within the system.
        /// Serves as the primary key for database operations and episode-specific references.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key reference to the parent TV show containing this episode.
        /// Establishes the hierarchical relationship and content organization structure.
        /// </summary>
        public int TvShowId { get; set; }

        /// <summary>
        /// Official title of the episode as it appears in broadcast and promotional materials.
        /// Represents the episode's unique identifier within the season context.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Season number within the TV show structure indicating episode grouping and chronological context.
        /// Provides organizational hierarchy for content navigation and viewing order.
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// Sequential episode number within the specific season context.
        /// Determines viewing order and episode position for proper content navigation.
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// Original broadcast or release date for the episode.
        /// Provides historical context and chronological reference for content organization.
        /// </summary>
        public DateTime AirDate { get; set; }

        /// <summary>
        /// Brief episode description or plot summary providing content context without major spoilers.
        /// Serves as content preview helping users make informed viewing decisions.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Quality rating for the episode on a standardized scale typically ranging from 0.0 to 10.0.
        /// Represents critical assessment or audience satisfaction for episode-specific quality measurement.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Navigation property establishing the relationship to the parent TV show entity.
        /// Enables bidirectional navigation and complex queries across show-episode relationships.
        /// </summary>
        public TvShow TvShow { get; set; }
    }
}