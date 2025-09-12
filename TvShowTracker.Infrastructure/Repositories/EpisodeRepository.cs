using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="Episode"/> entities in the database.
    /// Provides methods for retrieving episodes by TV show, season, and recent episodes.
    /// </summary>
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodeRepository"/> class.
        /// </summary>
        /// <param name="context">The <see cref="TvShowContext"/> instance used for database access.</param>
        public EpisodeRepository(TvShowContext context) : base(context) { }

        /// <summary>
        /// Retrieves all episodes for a given TV show, ordered by season and episode number.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show.</param>
        /// <returns>A collection of <see cref="Episode"/> entities.</returns>
        public async Task<IEnumerable<Episode>> GetByTvShowIdAsync(int tvShowId)
        {
            return await _dbSet
                .Where(e => e.TvShowId == tvShowId)
                .OrderBy(e => e.Season)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all episodes for a given TV show and season, ordered by episode number.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show.</param>
        /// <param name="season">The season number.</param>
        /// <returns>A collection of <see cref="Episode"/> entities.</returns>
        public async Task<IEnumerable<Episode>> GetBySeasonAsync(int tvShowId, int season)
        {
            return await _dbSet
                .Where(e => e.TvShowId == tvShowId && e.Season == season)
                .OrderBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single episode for a given TV show, season, and episode number.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show.</param>
        /// <param name="season">The season number.</param>
        /// <param name="episodeNumber">The episode number.</param>
        /// <returns>The <see cref="Episode"/> entity if found; otherwise, null.</returns>
        public async Task<Episode?> GetByTvShowAndEpisodeNumberAsync(int tvShowId, int season, int episodeNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.TvShowId == tvShowId &&
                                         e.Season == season &&
                                         e.EpisodeNumber == episodeNumber);
        }

        /// <summary>
        /// Retrieves the most recent episodes across all TV shows, including their TV show details.
        /// </summary>
        /// <param name="count">The maximum number of episodes to retrieve. Default is 10.</param>
        /// <returns>A collection of <see cref="Episode"/> entities.</returns>
        public async Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10)
        {
            return await _dbSet
                .Include(e => e.TvShow)
                .OrderByDescending(e => e.AirDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
