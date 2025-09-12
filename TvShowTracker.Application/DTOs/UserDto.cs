namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing user account information and basic profile data.
    /// Provides essential user details for display purposes while excluding sensitive authentication data.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Unique system identifier for the user account.
        /// Used for internal referencing and establishing relationships with user-generated content.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user's chosen username for identification and display purposes.
        /// Serves as the primary user identifier visible to other users and in public contexts.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The user's email address used for account verification and communication.
        /// Provides a means for account recovery and system notifications.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp indicating when the user account was originally created.
        /// Provides account age information and historical context for user activity.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Total count of TV shows the user has marked as favorites.
        /// Provides immediate access to user engagement metrics without additional API calls.
        /// </summary>
        public int FavoritesCount { get; set; }
    }
}