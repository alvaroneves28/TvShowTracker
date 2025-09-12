using TvShowTracker.Core.Entities;

namespace TvShowTracker.Core.Interfaces
{
    /// <summary>
    /// Repository interface for TV show-specific data access operations extending base repository functionality.
    /// Provides specialized querying capabilities for complex show retrieval, content discovery, and relationship management.
    /// </summary>
    public interface ITvShowRepository : IRepository<TvShow>
    {
        /// <summary>
        /// Retrieves all TV shows with their complete episode collections eagerly loaded.
        /// Provides comprehensive show data for scenarios requiring complete content information.
        /// </summary>
        /// <returns>Collection of TV shows with episodes included</returns>
        Task<IEnumerable<TvShow>> GetAllWithEpisodesAsync();

        /// <summary>
        /// Retrieves a specific TV show with its complete episode collection eagerly loaded.
        /// Provides comprehensive show data for detailed content display and user interaction.
        /// </summary>
        /// <param name="id">Unique identifier of the TV show to retrieve</param>
        /// <returns>TV show with episodes included, or null if not found</returns>
        Task<TvShow?> GetByIdWithEpisodesAsync(int id);

        /// <summary>
        /// Retrieves a specific TV show with its complete actor collection eagerly loaded.
        /// Provides comprehensive cast information for show detail and discovery scenarios.
        /// </summary>
        /// <param name="id">Unique identifier of the TV show to retrieve</param>
        /// <returns>TV show with actors included, or null if not found</returns>
        Task<TvShow?> GetByIdWithActorsAsync(int id);

        /// <summary>
        /// Retrieves TV shows filtered by specific genre classification.
        /// Provides genre-based content discovery and categorization functionality.
        /// </summary>
        /// <param name="genre">Genre name for filtering shows (case-insensitive)</param>
        /// <returns>Collection of shows belonging to the specified genre</returns>
        Task<IEnumerable<TvShow>> GetByGenreAsync(string genre);

        /// <summary>
        /// Retrieves TV shows filtered by content type classification.
        /// Enables format-based content discovery and specialized browsing experiences.
        /// </summary>
        /// <param name="type">Show type for filtering (Series, Miniseries, Documentary, etc.)</param>
        /// <returns>Collection of shows matching the specified type classification</returns>
        Task<IEnumerable<TvShow>> GetByTypeAsync(string type);

        /// <summary>
        /// Performs text-based search across TV show names and metadata.
        /// Provides comprehensive content discovery through flexible text matching.
        /// </summary>
        /// <param name="name">Search query for matching against show names and related content</param>
        /// <returns>Collection of shows matching the search criteria ordered by relevance</returns>
        Task<IEnumerable<TvShow>> SearchByNameAsync(string name);

        /// <summary>
        /// Retrieves a paginated collection of TV shows with comprehensive filtering, sorting, and search capabilities.
        /// Provides flexible content discovery with performance optimization for large catalogs.
        /// </summary>
        /// <param name="page">Page number for pagination (1-based)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="sortBy">Property name for sorting (default: Name)</param>
        /// <param name="sortDescending">Sort direction flag (default: ascending)</param>
        /// <param name="genre">Optional genre filter for content categorization</param>
        /// <param name="type">Optional type filter for format-based discovery</param>
        /// <param name="search">Optional search query for text-based content discovery</param>
        /// <returns>Tuple containing paginated shows and total count for pagination metadata</returns>
        Task<(IEnumerable<TvShow> TvShows, int TotalCount)> GetPagedAsync(
            int page, int pageSize, string? sortBy = null, bool sortDescending = false,
            string? genre = null, string? type = null, string? search = null);
    }
}