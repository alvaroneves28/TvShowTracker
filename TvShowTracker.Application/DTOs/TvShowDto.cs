namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing the core information for a TV show.
    /// Serves as the base representation for TV show data across API responses and client interfaces.
    /// </summary>
    public class TvShowDto
    {
        /// <summary>
        /// Unique system identifier for the TV show record.
        /// Used for referencing the show in API operations and establishing relationships.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The official title of the TV show as recognized in media and promotional materials.
        /// Represents the primary identifier used for show recognition and display.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Comprehensive description of the show's premise, setting, and overall narrative.
        /// Provides users with essential context for understanding the show's content and appeal.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The original premiere date when the show first aired or was released.
        /// Provides historical context and chronological reference for content organization.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Current production and broadcast status of the TV show.
        /// Indicates whether new episodes are expected and the show's lifecycle stage.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Primary broadcaster or streaming platform that originally distributes the show.
        /// Identifies the content source and may indicate production quality and style.
        /// </summary>
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// URL pointing to the official poster or key art image for the TV show.
        /// Provides visual identification and enhances user interface presentation.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Overall quality rating for the TV show on a 0.0 to 10.0 scale.
        /// Represents aggregated assessment of the show's quality and viewer satisfaction.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Collection of genre classifications describing the show's content themes and style.
        /// Enables categorization, filtering, and content discovery functionality.
        /// </summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// Classification of the show's format and production structure.
        /// Indicates the intended format and helps set appropriate user expectations.
        /// </summary>
        public string ShowType { get; set; } = string.Empty;

        /// <summary>
        /// Total number of episodes available across all seasons of the show.
        /// Provides users with content volume information for viewing decisions.
        /// </summary>
        public int EpisodeCount { get; set; }
    }
}