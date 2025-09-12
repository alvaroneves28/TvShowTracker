namespace TvShowTracker.Core.Entities
{
    /// <summary>
    /// Junction entity representing the many-to-many relationship between users and their favorite TV shows.
    /// Captures user preferences with temporal context and serves as the foundation for personalized content experiences.
    /// </summary>
    public class UserFavorite
    {
        /// <summary>
        /// Foreign key reference to the user who marked the show as favorite.
        /// Forms half of the composite primary key identifying unique user-show relationships.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Foreign key reference to the TV show that was marked as favorite.
        /// Forms the second component of the composite primary key for unique relationship identification.
        /// </summary>
        public int TvShowId { get; set; }

        /// <summary>
        /// Timestamp indicating when the user added this show to their favorites.
        /// Provides temporal context for user preference evolution and engagement analysis.
        /// </summary>
        public DateTime AddedAt { get; set; }

        /// <summary>
        /// Navigation property providing bidirectional access to the user entity.
        /// Enables complex queries and data loading scenarios requiring complete user context.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Navigation property providing bidirectional access to the TV show entity.
        /// Enables comprehensive favorite displays with complete show information and metadata.
        /// </summary>
        public TvShow TvShow { get; set; }
    }
}