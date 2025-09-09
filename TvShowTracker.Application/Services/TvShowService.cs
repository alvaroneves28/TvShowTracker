using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;

namespace TvShowTracker.Application.Services
{
    public class TvShowService : ITvShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TvShowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<TvShowDto>> GetAllTvShowsAsync(QueryParameters parameters)
        {
            // Validar parâmetros
            parameters.Page = Math.Max(1, parameters.Page);
            parameters.PageSize = Math.Clamp(parameters.PageSize, 1, 50);

            var (tvShows, totalCount) = await _unitOfWork.TvShows.GetPagedAsync(
                parameters.Page,
                parameters.PageSize,
                parameters.SortBy,
                parameters.SortDescending,
                parameters.Genre,
                parameters.Type,
                parameters.Search);

            var tvShowDtos = _mapper.Map<IEnumerable<TvShowDto>>(tvShows);

            return new PagedResultDto<TvShowDto>(tvShowDtos, parameters.Page, parameters.PageSize, totalCount);
        }

        public async Task<TvShowDetailDto?> GetTvShowByIdAsync(int id, int? userId = null)
        {
            var tvShow = await _unitOfWork.TvShows.GetByIdWithEpisodesAsync(id);
            if (tvShow == null) return null;

            // Carregar atores separadamente
            var tvShowWithActors = await _unitOfWork.TvShows.GetByIdWithActorsAsync(id);

            var detailDto = _mapper.Map<TvShowDetailDto>(tvShow);
            if (tvShowWithActors?.Actors != null)
            {
                detailDto.Actors = _mapper.Map<List<ActorDto>>(tvShowWithActors.Actors);
            }

            // Verificar se é favorito do usuário
            if (userId.HasValue)
            {
                detailDto.IsFavorite = await _unitOfWork.UserFavorites.IsFavoriteAsync(userId.Value, id);
            }

            return detailDto;
        }

        public async Task<TvShowDto> CreateTvShowAsync(CreateTvShowDto createDto)
        {
            var tvShow = _mapper.Map<TvShow>(createDto);
            tvShow.CreatedAt = DateTime.UtcNow;
            tvShow.UpdatedAt = DateTime.UtcNow;

            var createdTvShow = await _unitOfWork.TvShows.AddAsync(tvShow);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TvShowDto>(createdTvShow);
        }

        public async Task<TvShowDto?> UpdateTvShowAsync(int id, CreateTvShowDto updateDto)
        {
            var existingTvShow = await _unitOfWork.TvShows.GetByIdAsync(id);
            if (existingTvShow == null) return null;

            _mapper.Map(updateDto, existingTvShow);
            existingTvShow.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.TvShows.UpdateAsync(existingTvShow);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TvShowDto>(existingTvShow);
        }

        public async Task<bool> DeleteTvShowAsync(int id)
        {
            var tvShow = await _unitOfWork.TvShows.GetByIdAsync(id);
            if (tvShow == null) return false;

            await _unitOfWork.TvShows.DeleteAsync(tvShow);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TvShowDto>> SearchTvShowsAsync(string searchTerm)
        {
            var tvShows = await _unitOfWork.TvShows.SearchByNameAsync(searchTerm);
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        public async Task<IEnumerable<TvShowDto>> GetTvShowsByGenreAsync(string genre)
        {
            var tvShows = await _unitOfWork.TvShows.GetByGenreAsync(genre);
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        public async Task<IEnumerable<TvShowDto>> GetTvShowsByTypeAsync(string type)
        {
            var tvShows = await _unitOfWork.TvShows.GetByTypeAsync(type);
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        public async Task<IEnumerable<EpisodeDto>> GetTvShowEpisodesAsync(int tvShowId)
        {
            var episodes = await _unitOfWork.Episodes.GetByTvShowIdAsync(tvShowId);
            return _mapper.Map<IEnumerable<EpisodeDto>>(episodes);
        }

        public async Task<IEnumerable<ActorDto>> GetTvShowActorsAsync(int tvShowId)
        {
            var tvShow = await _unitOfWork.TvShows.GetByIdWithActorsAsync(tvShowId);
            return _mapper.Map<IEnumerable<ActorDto>>(tvShow?.Actors ?? new List<Actor>());
        }
    }
}
