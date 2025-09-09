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
    public class EpisodeRepository : Repository<Episode>, IEpisodeRepository
    {
        public EpisodeRepository(TvShowContext context) : base(context) { }

        public async Task<IEnumerable<Episode>> GetByTvShowIdAsync(int tvShowId)
        {
            return await _dbSet
                .Where(e => e.TvShowId == tvShowId)
                .OrderBy(e => e.Season)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Episode>> GetBySeasonAsync(int tvShowId, int season)
        {
            return await _dbSet
                .Where(e => e.TvShowId == tvShowId && e.Season == season)
                .OrderBy(e => e.EpisodeNumber)
                .ToListAsync();
        }

        public async Task<Episode?> GetByTvShowAndEpisodeNumberAsync(int tvShowId, int season, int episodeNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.TvShowId == tvShowId &&
                                         e.Season == season &&
                                         e.EpisodeNumber == episodeNumber);
        }

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
