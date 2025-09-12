using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="UserFavorite"/> entities.
    /// Provides methods to retrieve, add, and remove user favorite TV shows.
    /// </summary>
    public class UserFavoriteRepository : IUserFavoriteRepository
    {
        private readonly TvShowContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFavoriteRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="TvShowContext"/> used for database access.</param>
        public UserFavoriteRepository(TvShowContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all favorite TV shows for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of <see cref="TvShow"/> entities marked as favorites by the user.</returns>
        public async Task<IEnumerable<TvShow>> GetUserFavoritesAsync(int userId)
        {
            return await _context.UserFavorites
                .Where(uf => uf.UserId == userId)
                .Include(uf => uf.TvShow)
                .Select(uf => uf.TvShow)
                .ToListAsync();
        }

        /// <summary>
        /// Checks whether a TV show is a favorite for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="tvShowId">The ID of the TV show.</param>
        /// <returns>True if the TV show is a favorite; otherwise, false.</returns>
        public async Task<bool> IsFavoriteAsync(int userId, int tvShowId)
        {
            return await _context.UserFavorites
                .AnyAsync(uf => uf.UserId == userId && uf.TvShowId == tvShowId);
        }

        /// <summary>
        /// Adds a TV show to a user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="tvShowId">The ID of the TV show.</param>
        public async Task AddFavoriteAsync(int userId, int tvShowId)
        {
            var favorite = new UserFavorite
            {
                UserId = userId,
                TvShowId = tvShowId,
                AddedAt = DateTime.UtcNow
            };

            await _context.UserFavorites.AddAsync(favorite);
        }

        /// <summary>
        /// Removes a TV show from a user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="tvShowId">The ID of the TV show.</param>
        public async Task RemoveFavoriteAsync(int userId, int tvShowId)
        {
            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.TvShowId == tvShowId);

            if (favorite != null)
            {
                _context.UserFavorites.Remove(favorite);
            }
        }

        /// <summary>
        /// Gets the total number of favorite TV shows for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The count of favorite TV shows.</returns>
        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            return await _context.UserFavorites
                .CountAsync(uf => uf.UserId == userId);
        }
    }
}
