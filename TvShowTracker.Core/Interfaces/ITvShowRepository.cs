using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    public interface ITvShowRepository : IRepository<TvShow>
    {
        Task<IEnumerable<TvShow>> GetAllWithEpisodesAsync();
        Task<TvShow?> GetByIdWithEpisodesAsync(int id);
        Task<TvShow?> GetByIdWithActorsAsync(int id);
        Task<IEnumerable<TvShow>> GetByGenreAsync(string genre);
        Task<IEnumerable<TvShow>> GetByTypeAsync(string type);
        Task<IEnumerable<TvShow>> SearchByNameAsync(string name);
        Task<(IEnumerable<TvShow> TvShows, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? sortBy = null, bool sortDescending = false,
            string? genre = null, string? type = null, string? search = null);
    }
}
