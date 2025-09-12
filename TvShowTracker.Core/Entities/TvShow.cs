namespace TvShowTracker.Core.Entities
{
    /// <summary>
    /// Core domain entity representing a TV show with comprehensive metadata and relationship management.
    /// Serves as the central aggregate root for all show-related information and entity relationships.
    /// </summary>
    public class TvShow
    {
        /// <summary>
        /// Unique identifier for the TV show entity serving as the primary key.
        /// Acts as the central reference point for all show-related operations and relationships.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Official title of the TV show as recognized in broadcast and promotional materials.
        /// Serves as the primary identifier for user recognition and content discovery.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Comprehensive description of the show's premise, setting, and narrative themes.
        /// Provides detailed context for user decision-making and content discovery.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Original premiere date when the show first aired or was released.
        /// Provides historical context and chronological organization for content catalog.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Current production and broadcast status indicating show lifecycle stage.
        /// Provides users with expectations about new content availability.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Primary broadcaster or streaming platform that originally distributes the show.
        /// Indicates content source and may influence content style and target audience.
        /// </summary>
        public string Network { get; set; }

        /// <summary>
        /// URL pointing to official poster or promotional artwork for the show.
        /// Provides visual identification and enhances user interface presentation.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Overall quality rating for the show on a standardized scale.
        /// Represents aggregated critical assessment and audience satisfaction measurement.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// Collection of genre classifications describing the show's content themes and style.
        /// Enables multi-dimensional content categorization and discovery functionality.
        /// </summary>
        public List<string> Genres { get; set; }

        /// <summary>
        /// Format classification indicating the show's structure and production approach.
        /// Helps users understand content commitment and viewing pattern expectations.
        /// </summary>
        public string ShowType { get; set; }

        /// <summary>
        /// Navigation property representing the complete collection of episodes for this show.
        /// Establishes the one-to-many relationship enabling comprehensive episode management.
        /// </summary>
        public List<Episode> Episodes { get; set; }

        /// <summary>
        /// Navigation property representing the many-to-many relationship with actors.
        /// Enables comprehensive cast management and actor-based content discovery.
        /// </summary>
        public List<Actor> Actors { get; set; }

        /// <summary>
        /// Navigation property representing user favorite relationships for this show.
        /// Enables user engagement tracking and personalized recommendation functionality.
        /// </summary>
        public List<UserFavorite> UserFavorites { get; set; }

        /// <summary>
        /// Timestamp indicating when the show record was originally created in the system.
        /// Provides audit trail and content lifecycle tracking for operational management.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Timestamp indicating when the show record was last modified in the system.
        /// Enables change tracking and data freshness assessment for content management.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}