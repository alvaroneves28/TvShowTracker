using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;

namespace TvShowTracker.Application.Interfaces
{
    public interface ITvShowService
    {
        Task<PagedResultDto<TvShowDto>> GetAllTvShowsAsync(QueryParameters parameters);
        Task<TvShowDetailDto?> GetTvShowByIdAsync(int id, int? userId = null);
        Task<TvShowDto> CreateTvShowAsync(CreateTvShowDto createDto);
        Task<TvShowDto?> UpdateTvShowAsync(int id, CreateTvShowDto updateDto);
        Task<bool> DeleteTvShowAsync(int id);
        Task<IEnumerable<TvShowDto>> SearchTvShowsAsync(string searchTerm);
        Task<IEnumerable<TvShowDto>> GetTvShowsByGenreAsync(string genre);
        Task<IEnumerable<TvShowDto>> GetTvShowsByTypeAsync(string type);
        Task<IEnumerable<EpisodeDto>> GetTvShowEpisodesAsync(int tvShowId);
        Task<IEnumerable<ActorDto>> GetTvShowActorsAsync(int tvShowId);
    }
}
