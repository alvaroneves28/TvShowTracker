using TvShowTracker.Application.DTOs;

namespace TvShowTracker.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing user favorite TV shows and related operations.
    /// Provides comprehensive functionality for favorite show management, querying, and analytics.
    /// </summary>
    public interface IFavoriteService
    {
        /// <summary>
        /// Retrieves all TV shows marked as favorites by a specific user.
        /// Returns a comprehensive collection of favorite shows with complete metadata.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose favorites to retrieve</param>
        /// <returns>Collection of TvShowDto objects representing the user's favorite shows</returns>
        Task<IEnumerable<TvShowDto>> GetUserFavoritesAsync(int userId);

        /// <summary>
        /// Adds a TV show to a user's favorites collection.
        /// Creates a new user-show relationship with timestamp tracking.
        /// </summary>
        /// <param name="userId">The unique identifier of the user adding the favorite</param>
        /// <param name="tvShowId">The unique identifier of the TV show to add as favorite</param>
        /// <returns>True if the show was successfully added, false if the operation failed</returns>
        Task<bool> AddFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Removes a TV show from a user's favorites collection.
        /// Deletes the existing user-show relationship while maintaining data integrity.
        /// </summary>
        /// <param name="userId">The unique identifier of the user removing the favorite</param>
        /// <param name="tvShowId">The unique identifier of the TV show to remove from favorites</param>
        /// <returns>True if the show was successfully removed, false if the operation failed</returns>
        Task<bool> RemoveFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Checks whether a specific TV show is marked as favorite by a user.
        /// Provides rapid favorite status lookup for user interface components.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check</param>
        /// <param name="tvShowId">The unique identifier of the TV show to check</param>
        /// <returns>True if the show is in the user's favorites, false otherwise</returns>
        Task<bool> IsFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Retrieves the total count of shows marked as favorites by a specific user.
        /// Provides user engagement statistics without requiring full data retrieval.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose favorite count to retrieve</param>
        /// <returns>The total number of shows in the user's favorites collection</returns>
        Task<int> GetFavoritesCountAsync(int userId);
    }
}