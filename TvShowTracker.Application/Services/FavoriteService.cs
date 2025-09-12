using AutoMapper;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Core.Interfaces;

namespace TvShowTracker.Application.Services
{
    /// <summary>
    /// Service implementation for managing user favorite TV shows and related operations.
    /// Provides comprehensive business logic for favorite management with data integrity and performance optimization.
    /// </summary>
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes the FavoriteService with required dependencies for data access and object mapping.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work instance for coordinated data access operations</param>
        /// <param name="mapper">AutoMapper instance for entity-to-DTO conversions</param>
        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all TV shows marked as favorites by a specific user.
        /// Returns mapped DTO objects with complete show information for display purposes.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose favorites to retrieve</param>
        /// <returns>Collection of TvShowDto objects representing the user's favorite shows</returns>
        public async Task<IEnumerable<TvShowDto>> GetUserFavoritesAsync(int userId)
        {
            // Delegate to repository for optimized data retrieval
            var favorites = await _unitOfWork.UserFavorites.GetUserFavoritesAsync(userId);

            // Use AutoMapper for consistent entity-to-DTO conversion
            return _mapper.Map<IEnumerable<TvShowDto>>(favorites);
        }

        /// <summary>
        /// Adds a TV show to a user's favorites collection with comprehensive validation.
        /// Ensures data integrity through show existence and duplicate prevention checks.
        /// </summary>
        /// <param name="userId">The unique identifier of the user adding the favorite</param>
        /// <param name="tvShowId">The unique identifier of the TV show to add as favorite</param>
        /// <returns>True if the show was successfully added, false if the operation failed due to validation</returns>
        public async Task<bool> AddFavoriteAsync(int userId, int tvShowId)
        {
            // Validate that the TV show exists before creating favorite relationship
            var tvShow = await _unitOfWork.TvShows.GetByIdAsync(tvShowId);
            if (tvShow == null)
                return false; // Cannot favorite non-existent show

            // Prevent duplicate favorites by checking existing relationship
            if (await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId))
                return false; // Favorite relationship already exists

            // Create favorite relationship and commit transaction
            await _unitOfWork.UserFavorites.AddFavoriteAsync(userId, tvShowId);
            await _unitOfWork.SaveChangesAsync();

            return true; // Favorite successfully added
        }

        /// <summary>
        /// Removes a TV show from a user's favorites collection with existence validation.
        /// Ensures operation safety through pre-removal validation checks.
        /// </summary>
        /// <param name="userId">The unique identifier of the user removing the favorite</param>
        /// <param name="tvShowId">The unique identifier of the TV show to remove from favorites</param>
        /// <returns>True if the show was successfully removed, false if the favorite relationship didn't exist</returns>
        public async Task<bool> RemoveFavoriteAsync(int userId, int tvShowId)
        {
            // Validate that favorite relationship exists before removal
            if (!await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId))
                return false; // Cannot remove non-existent favorite

            // Remove favorite relationship and commit transaction
            await _unitOfWork.UserFavorites.RemoveFavoriteAsync(userId, tvShowId);
            await _unitOfWork.SaveChangesAsync();

            return true; // Favorite successfully removed
        }

        /// <summary>
        /// Checks whether a specific TV show is marked as favorite by a user.
        /// Provides efficient favorite status lookup for UI state management.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check</param>
        /// <param name="tvShowId">The unique identifier of the TV show to check</param>
        /// <returns>True if the show is in the user's favorites, false otherwise</returns>
        public async Task<bool> IsFavoriteAsync(int userId, int tvShowId)
        {
            // Direct delegation to repository for optimized existence checking
            return await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId);
        }

        /// <summary>
        /// Retrieves the total count of shows marked as favorites by a specific user.
        /// Provides efficient aggregation for user engagement statistics and profile information.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose favorite count to retrieve</param>
        /// <returns>The total number of shows in the user's favorites collection</returns>
        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            // Direct delegation to repository for optimized count aggregation
            return await _unitOfWork.UserFavorites.GetFavoritesCountAsync(userId);
        }
    }
}