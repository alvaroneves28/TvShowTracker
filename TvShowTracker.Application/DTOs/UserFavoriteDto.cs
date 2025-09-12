namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing the relationship between a user and their favorite TV show.
    /// Captures the many-to-many association with temporal context and optional denormalized data.
    /// </summary>
    public class UserFavoriteDto
    {
        /// <summary>
        /// Foreign key reference to the user who marked the show as favorite.
        /// Establishes the user side of the many-to-many relationship.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Foreign key reference to the TV show that was marked as favorite.
        /// Establishes the show side of the many-to-many relationship.
        /// </summary>
        public int TvShowId { get; set; }

        /// <summary>
        /// Timestamp indicating when the user added this show to their favorites.
        /// Provides temporal context for user preference evolution and engagement tracking.
        /// </summary>
        public DateTime AddedAt { get; set; }

        /// <summary>
        /// Optional user information associated with this favorite relationship.
        /// Provides denormalized user data to reduce API calls in certain display scenarios.
        /// </summary>
        public UserDto? User { get; set; }

        /// <summary>
        /// Optional TV show information associated with this favorite relationship.
        /// Provides denormalized show data to reduce API calls in certain display scenarios.
        /// </summary>
        public TvShowDto? TvShow { get; set; }
    }
}