using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Repository interface for episode-specific data access operations extending base repository functionality.
    /// Provides specialized querying capabilities for episode retrieval, organization, and content discovery.
    /// </summary>
    public interface IEpisodeRepository : IRepository<Episode>
    {
        /// <summary>
        /// Retrieves all episodes for a specific TV show ordered by season and episode number.
        /// Provides complete episode collection for comprehensive show content display and navigation.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show whose episodes to retrieve</param>
        /// <returns>Collection of episodes ordered chronologically by season and episode number</returns>
        Task<IEnumerable<Episode>> GetByTvShowIdAsync(int tvShowId);

        /// <summary>
        /// Retrieves all episodes for a specific season within a TV show.
        /// Enables season-based content navigation and viewing organization for user interfaces.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show containing the target season</param>
        /// <param name="season">Season number to retrieve episodes for</param>
        /// <returns>Collection of episodes for the specified season ordered by episode number</returns>
        Task<IEnumerable<Episode>> GetBySeasonAsync(int tvShowId, int season);

        /// <summary>
        /// Retrieves a specific episode using show identifier and episode coordinates.
        /// Provides precise episode lookup through hierarchical identification for direct content access.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show containing the target episode</param>
        /// <param name="season">Season number containing the target episode</param>
        /// <param name="episodeNumber">Episode number within the specified season</param>
        /// <returns>Specific episode matching the coordinates, or null if not found</returns>
        Task<Episode?> GetByTvShowAndEpisodeNumberAsync(int tvShowId, int season, int episodeNumber);

        /// <summary>
        /// Retrieves recently aired or added episodes for content discovery and user engagement.
        /// Provides temporal episode filtering for "what's new" functionality and content freshness indication.
        /// </summary>
        /// <param name="count">Maximum number of recent episodes to retrieve (default: 10)</param>
        /// <returns>Collection of recently aired episodes ordered by air date (newest first)</returns>
        Task<IEnumerable<Episode>> GetRecentEpisodesAsync(int count = 10);
    }
}