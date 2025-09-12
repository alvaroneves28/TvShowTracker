using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Repository interface for user-specific data access operations extending base repository functionality.
    /// Provides specialized querying capabilities for user authentication, account management, and security operations.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Retrieves a user by their unique username identifier.
        /// Provides username-based user lookup for authentication and profile access scenarios.
        /// </summary>
        /// <param name="username">Username of the user to retrieve</param>
        /// <returns>User entity with matching username, or null if not found</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user by their email address identifier.
        /// Provides email-based user lookup for authentication and account recovery scenarios.
        /// </summary>
        /// <param name="email">Email address of the user to retrieve</param>
        /// <returns>User entity with matching email, or null if not found</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Checks whether a username is already registered in the system.
        /// Provides efficient username availability validation for registration workflows.
        /// </summary>
        /// <param name="username">Username to check for existing registration</param>
        /// <returns>True if username exists, false if available for registration</returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Checks whether an email address is already registered in the system.
        /// Provides efficient email availability validation for registration and account management.
        /// </summary>
        /// <param name="email">Email address to check for existing registration</param>
        /// <returns>True if email exists, false if available for registration</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}