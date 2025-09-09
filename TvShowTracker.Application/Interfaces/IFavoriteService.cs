using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;

namespace TvShowTracker.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<IEnumerable<TvShowDto>> GetUserFavoritesAsync(int userId);
        Task<bool> AddFavoriteAsync(int userId, int tvShowId);
        Task<bool> RemoveFavoriteAsync(int userId, int tvShowId);
        Task<bool> IsFavoriteAsync(int userId, int tvShowId);
        Task<int> GetFavoritesCountAsync(int userId);
    }
}
