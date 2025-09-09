using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    public class UserFavoriteRepository : IUserFavoriteRepository
    {
        private readonly TvShowContext _context;

        public UserFavoriteRepository(TvShowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TvShow>> GetUserFavoritesAsync(int userId)
        {
            return await _context.UserFavorites
                .Where(uf => uf.UserId == userId)
                .Include(uf => uf.TvShow)
                .Select(uf => uf.TvShow)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(int userId, int tvShowId)
        {
            return await _context.UserFavorites
                .AnyAsync(uf => uf.UserId == userId && uf.TvShowId == tvShowId);
        }

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

        public async Task RemoveFavoriteAsync(int userId, int tvShowId)
        {
            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.TvShowId == tvShowId);

            if (favorite != null)
            {
                _context.UserFavorites.Remove(favorite);
            }
        }

        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            return await _context.UserFavorites
                .CountAsync(uf => uf.UserId == userId);
        }
    }
}
