using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Infrastructure.ExternalServices.Models;

namespace TvShowTracker.Infrastructure.ExternalServices
{
    public interface IExternalTvShowService
    {
        Task<EpisodateResponse?> GetPopularShowsAsync(int page = 1);
        Task<EpisodateTvShowDetail?> GetShowDetailsAsync(int showId);
        Task<List<EpisodateTvShow>> GetAllPopularShowsAsync(int maxPages = 5);
    }
}
