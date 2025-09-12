namespace TvShowTracker.Core.Entities
{
    /// <summary>
    /// Domain entity representing a user account with authentication credentials and profile information.
    /// Serves as the foundation for user identity, authentication, and personalized content experiences.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user account serving as the primary key.
        /// Establishes user identity across all system operations and relationships.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique username chosen by the user for identification and display purposes.
        /// Serves as the primary user identifier visible to other users and in public contexts.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's email address for account verification, communication, and account recovery.
        /// Serves as a primary communication channel and alternative authentication identifier.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Cryptographically hashed password for secure authentication.
        /// Stores the secure hash of the user's password using industry-standard hashing algorithms.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Timestamp indicating when the user account was originally created.
        /// Provides account age information and audit trail for user lifecycle management.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property representing the user's favorite TV shows collection.
        /// Establishes the one-to-many relationship enabling personalized content experiences.
        /// </summary>
        public List<UserFavorite> Favorites { get; set; }
    }
}