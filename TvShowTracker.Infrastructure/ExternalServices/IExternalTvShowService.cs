using TvShowTracker.Infrastructure.ExternalServices.Models;

namespace TvShowTracker.Infrastructure.ExternalServices
{
    /// <summary>
    /// Defines the contract for an external TV show service.
    /// Provides methods to retrieve popular shows, detailed show information, and multiple pages of shows.
    /// </summary>
    public interface IExternalTvShowService
    {
        /// <summary>
        /// Retrieves a page of the most popular TV shows from the external API.
        /// </summary>
        /// <param name="page">The page number to retrieve. Default is 1.</param>
        /// <returns>An <see cref="EpisodateResponse"/> containing the list of TV shows, or null if an error occurs.</returns>
        Task<EpisodateResponse?> GetPopularShowsAsync(int page = 1);

        /// <summary>
        /// Retrieves detailed information about a specific TV show by its ID.
        /// </summary>
        /// <param name="showId">The ID of the TV show.</param>
        /// <returns>An <see cref="EpisodateTvShowDetail"/> with detailed information, or null if an error occurs.</returns>
        Task<EpisodateTvShowDetail?> GetShowDetailsAsync(int showId);

        /// <summary>
        /// Retrieves multiple pages of popular TV shows from the external API, up to a specified maximum number of pages.
        /// </summary>
        /// <param name="maxPages">The maximum number of pages to retrieve. Default is 5.</param>
        /// <returns>A list of <see cref="EpisodateTvShow"/> representing all retrieved popular shows.</returns>
        Task<List<EpisodateTvShow>> GetAllPopularShowsAsync(int maxPages = 5);
    }
}
