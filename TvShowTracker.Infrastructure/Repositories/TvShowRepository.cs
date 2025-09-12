using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="TvShow"/> entities in the database.
    /// Provides methods to retrieve TV shows with episodes, actors, and apply filtering, sorting, and pagination.
    /// </summary>
    public class TvShowRepository : Repository<TvShow>, ITvShowRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="TvShowContext"/> instance used for database access.</param>
        public TvShowRepository(TvShowContext context) : base(context) { }

        /// <summary>
        /// Retrieves all TV shows including their episodes.
        /// </summary>
        /// <returns>A collection of <see cref="TvShow"/> entities with episodes.</returns>
        public async Task<IEnumerable<TvShow>> GetAllWithEpisodesAsync()
        {
            return await _dbSet
                .Include(ts => ts.Episodes)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a TV show by its ID, including its episodes.
        /// </summary>
        /// <param name="id">The ID of the TV show.</param>
        /// <returns>The <see cref="TvShow"/> entity if found; otherwise, null.</returns>
        public async Task<TvShow?> GetByIdWithEpisodesAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Episodes)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        /// <summary>
        /// Retrieves a TV show by its ID, including its actors.
        /// </summary>
        /// <param name="id">The ID of the TV show.</param>
        /// <returns>The <see cref="TvShow"/> entity if found; otherwise, null.</returns>
        public async Task<TvShow?> GetByIdWithActorsAsync(int id)
        {
            return await _dbSet
                .Include(ts => ts.Actors)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }

        /// <summary>
        /// Retrieves TV shows that belong to a specific genre.
        /// </summary>
        /// <param name="genre">The genre to filter by.</param>
        /// <returns>A collection of <see cref="TvShow"/> entities.</returns>
        public async Task<IEnumerable<TvShow>> GetByGenreAsync(string genre)
        {
            return await _dbSet
                .Where(ts => ts.Genres.Contains(genre))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves TV shows of a specific type.
        /// </summary>
        /// <param name="type">The type of TV show (e.g., "Series", "Movie").</param>
        /// <returns>A collection of <see cref="TvShow"/> entities.</returns>
        public async Task<IEnumerable<TvShow>> GetByTypeAsync(string type)
        {
            return await _dbSet
                .Where(ts => ts.ShowType == type)
                .ToListAsync();
        }

        /// <summary>
        /// Searches TV shows by name.
        /// </summary>
        /// <param name="name">The search term.</param>
        /// <returns>A collection of <see cref="TvShow"/> entities whose names contain the search term.</returns>
        public async Task<IEnumerable<TvShow>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(ts => ts.Name.Contains(name))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a paged list of TV shows with optional filtering, sorting, and search.
        /// </summary>
        /// <param name="page">The page number (starting from 1).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="sortBy">Optional property to sort by (name, rating, startDate, createdAt).</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="genre">Optional genre filter.</param>
        /// <param name="type">Optional type filter.</param>
        /// <param name="search">Optional search term for name or description.</param>
        /// <returns>A tuple containing the list of <see cref="TvShow"/> entities and the total count before pagination.</returns>
        public async Task<(IEnumerable<TvShow> TvShows, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? sortBy = null, bool sortDescending = false,
            string? genre = null, string? type = null, string? search = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(genre))
                query = query.Where(ts => ts.Genres.Contains(genre));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(ts => ts.ShowType == type);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(ts => ts.Name.Contains(search) || ts.Description.Contains(search));

            // Count total before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "name" => sortDescending ? query.OrderByDescending(ts => ts.Name) : query.OrderBy(ts => ts.Name),
                "rating" => sortDescending ? query.OrderByDescending(ts => ts.Rating) : query.OrderBy(ts => ts.Rating),
                "startdate" => sortDescending ? query.OrderByDescending(ts => ts.StartDate) : query.OrderBy(ts => ts.StartDate),
                "createdat" => sortDescending ? query.OrderByDescending(ts => ts.CreatedAt) : query.OrderBy(ts => ts.CreatedAt),
                _ => query.OrderBy(ts => ts.Name)
            };

            // Apply pagination
            var tvShows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tvShows, totalCount);
        }
    }
}
