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
    public class TvShowRepository : Repository<TvShow>, ITvShowRepository
    {
        public TvShowRepository(TvShowContext context) : base(context) { }

        public async Task<IEnumerable<TvShow>> GetAllWithEpisodesAsync()
        {
            return await _dbSet
                .Include(ts => ts.Episodes)
                .ToListAsync();
        }

        public async Task<TvShow?> GetByIdWithEpisodesAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Episodes)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<TvShow?> GetByIdWithActorsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Actors)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        public async Task<IEnumerable<TvShow>> GetByGenreAsync(string genre)
        {
            return await _dbSet
                .Where(ts => ts.Genres.Contains(genre))
                .ToListAsync();
        }

        public async Task<IEnumerable<TvShow>> GetByTypeAsync(string type)
        {
            return await _dbSet
                .Where(ts => ts.ShowType == type)
                .ToListAsync();
        }

        public async Task<IEnumerable<TvShow>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(ts => ts.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<(IEnumerable<TvShow> TvShows, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? sortBy = null, bool sortDescending = false,
            string? genre = null, string? type = null, string? search = null)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(genre))
                query = query.Where(ts => ts.Genres.Contains(genre));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(ts => ts.ShowType == type);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(ts => ts.Name.Contains(search) || ts.Description.Contains(search));

            // Contar total (antes da paginação)
            var totalCount = await query.CountAsync();

            // Aplicar ordenação
            query = sortBy?.ToLower() switch
            {
                "name" => sortDescending ? query.OrderByDescending(ts => ts.Name) : query.OrderBy(ts => ts.Name),
                "rating" => sortDescending ? query.OrderByDescending(ts => ts.Rating) : query.OrderBy(ts => ts.Rating),
                "startdate" => sortDescending ? query.OrderByDescending(ts => ts.StartDate) : query.OrderBy(ts => ts.StartDate),
                "createdat" => sortDescending ? query.OrderByDescending(ts => ts.CreatedAt) : query.OrderBy(ts => ts.CreatedAt),
                _ => query.OrderBy(ts => ts.Name)
            };

            // Aplicar paginação
            var tvShows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tvShows, totalCount);
        }
    }
}
