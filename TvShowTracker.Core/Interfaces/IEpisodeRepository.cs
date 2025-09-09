using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>
    {
        Task<IEnumerable<Episode>> GetByTvShowIdAsync(int tvShowId);
        Task<IEnumerable<Episode>> GetBySeasonAsync(int tvShowId, int season);
        Task<Episode?> GetByTvShowAndEpisodeNumberAsync(int tvShowId, int season, int episodeNumber);
        Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10);
    }
}
