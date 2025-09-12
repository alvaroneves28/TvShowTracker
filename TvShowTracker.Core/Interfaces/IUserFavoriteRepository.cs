using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Repository interface for user favorite relationship management and personalized content operations.
    /// Provides specialized data access methods for user preference tracking and engagement analytics.
    /// </summary>
    public interface IUserFavoriteRepository
    {
        /// <summary>
        /// Retrieves all TV shows marked as favorites by a specific user.
        /// Returns complete show information ordered by the date added to favorites.
        /// </summary>
        /// <param name="userId">Unique identifier of the user whose favorites to retrieve</param>
        /// <returns>Collection of TV shows in the user's favorites list</returns>
        Task<IEnumerable<TvShow>> GetUserFavoritesAsync(int userId);

        /// <summary>
        /// Checks whether a specific TV show is marked as favorite by a user.
        /// Provides efficient existence checking for real-time user interface updates.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check</param>
        /// <param name="tvShowId">Unique identifier of the TV show to check</param>
        /// <returns>True if the show is in the user's favorites, false otherwise</returns>
        Task<bool> IsFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Creates a new favorite relationship between a user and TV show.
        /// Establishes the many-to-many relationship with timestamp tracking for analytics.
        /// </summary>
        /// <param name="userId">Unique identifier of the user adding the favorite</param>
        /// <param name="tvShowId">Unique identifier of the TV show to mark as favorite</param>
        /// <returns>Task representing the asynchronous favorite creation operation</returns>
        Task AddFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Removes an existing favorite relationship between a user and TV show.
        /// Deletes the many-to-many relationship while preserving historical analytics data.
        /// </summary>
        /// <param name="userId">Unique identifier of the user removing the favorite</param>
        /// <param name="tvShowId">Unique identifier of the TV show to remove from favorites</param>
        /// <returns>Task representing the asynchronous favorite removal operation</returns>
        Task RemoveFavoriteAsync(int userId, int tvShowId);

        /// <summary>
        /// Retrieves the total count of shows marked as favorites by a specific user.
        /// Provides efficient aggregation for user engagement statistics and profile information.
        /// </summary>
        /// <param name="userId">Unique identifier of the user whose favorite count to retrieve</param>
        /// <returns>Total number of shows in the user's favorites collection</returns>
        Task<int> GetFavoritesCountAsync(int userId);
    }
}