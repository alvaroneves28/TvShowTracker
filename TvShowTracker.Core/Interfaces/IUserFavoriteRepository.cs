using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    public interface IUserFavoriteRepository
    {
        Task<IEnumerable<TvShow>> GetUserFavoritesAsync(int userId);
        Task<bool> IsFavoriteAsync(int userId, int tvShowId);
        Task AddFavoriteAsync(int userId, int tvShowId);
        Task RemoveFavoriteAsync(int userId, int tvShowId);
        Task<int> GetFavoritesCountAsync(int userId);
    }
}
