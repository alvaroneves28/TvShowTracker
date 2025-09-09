using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Core.Interfaces;

namespace TvShowTracker.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TvShowDto>> GetUserFavoritesAsync(int userId)
        {
            var favorites = await _unitOfWork.UserFavorites.GetUserFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<TvShowDto>>(favorites);
        }

        public async Task<bool> AddFavoriteAsync(int userId, int tvShowId)
        {
            // Verificar se a série existe
            var tvShow = await _unitOfWork.TvShows.GetByIdAsync(tvShowId);
            if (tvShow == null) return false;

            // Verificar se já é favorito
            if (await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId))
                return false;

            await _unitOfWork.UserFavorites.AddFavoriteAsync(userId, tvShowId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, int tvShowId)
        {
            if (!await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId))
                return false;

            await _unitOfWork.UserFavorites.RemoveFavoriteAsync(userId, tvShowId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsFavoriteAsync(int userId, int tvShowId)
        {
            return await _unitOfWork.UserFavorites.IsFavoriteAsync(userId, tvShowId);
        }

        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            return await _unitOfWork.UserFavorites.GetFavoritesCountAsync(userId);
        }
    }
}
