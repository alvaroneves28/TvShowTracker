using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;

namespace TvShowTracker.Application.Interfaces
{
    /// <summary>
    /// Service interface for comprehensive TV show management and data operations.
    /// Provides the primary business logic layer for TV show CRUD operations, search functionality, 
    /// and related entity management within the application.
    /// </summary>
    public interface ITvShowService
    {
        /// <summary>
        /// Retrieves a paginated collection of TV shows with comprehensive filtering, sorting, and search capabilities.
        /// Supports complex querying scenarios for efficient show catalog browsing and discovery.
        /// </summary>
        /// <param name="parameters">Query parameters containing pagination, filtering, sorting, and search criteria</param>
        /// <returns>Paginated result containing show data and pagination metadata</returns>
        Task<PagedResultDto<TvShowDto>> GetAllTvShowsAsync(QueryParameters parameters);

        /// <summary>
        /// Retrieves comprehensive details for a specific TV show including related entities and user context.
        /// Provides complete show information optimized for detailed view pages and user interaction.
        /// </summary>
        /// <param name="id">Unique identifier of the TV show to retrieve</param>
        /// <param name="userId">Optional user identifier for personalized data inclusion</param>
        /// <returns>Detailed show information with related entities, or null if show not found</returns>
        Task<TvShowDetailDto?> GetTvShowByIdAsync(int id, int? userId = null);

        /// <summary>
        /// Creates a new TV show entry in the system with comprehensive validation and metadata.
        /// Establishes a complete show record ready for episode and cast association.
        /// </summary>
        /// <param name="createDto">Complete show creation data including metadata and classification</param>
        /// <returns>Created show data with system-generated identifiers and timestamps</returns>
        Task<TvShowDto> CreateTvShowAsync(CreateTvShowDto createDto);

        /// <summary>
        /// Updates an existing TV show with new information while maintaining data integrity.
        /// Applies comprehensive validation and handles related entity synchronization.
        /// </summary>
        /// <param name="id">Unique identifier of the show to update</param>
        /// <param name="updateDto">Updated show data with new metadata and classification</param>
        /// <returns>Updated show data reflecting changes, or null if show not found</returns>
        Task<TvShowDto?> UpdateTvShowAsync(int id, CreateTvShowDto updateDto);

        /// <summary>
        /// Removes a TV show and all associated data from the system with proper cleanup.
        /// Handles cascade deletion and maintains data integrity across related entities.
        /// </summary>
        /// <param name="id">Unique identifier of the show to delete</param>
        /// <returns>True if deletion was successful, false if show was not found</returns>
        Task<bool> DeleteTvShowAsync(int id);

        /// <summary>
        /// Performs comprehensive text-based search across TV show content and metadata.
        /// Provides intelligent search results with relevance ranking and fuzzy matching.
        /// </summary>
        /// <param name="searchTerm">Text query for searching show names, descriptions, and related content</param>
        /// <returns>Collection of shows matching the search criteria, ordered by relevance</returns>
        Task<IEnumerable<TvShowDto>> SearchTvShowsAsync(string searchTerm);

        /// <summary>
        /// Retrieves TV shows filtered by specific genre classification.
        /// Provides efficient genre-based content discovery and browsing capabilities.
        /// </summary>
        /// <param name="genre">Genre name for filtering shows (case-insensitive matching)</param>
        /// <returns>Collection of shows belonging to the specified genre</returns>
        Task<IEnumerable<TvShowDto>> GetTvShowsByGenreAsync(string genre);

        /// <summary>
        /// Retrieves TV shows filtered by content type classification.
        /// Enables format-based content discovery and specialized browsing experiences.
        /// </summary>
        /// <param name="type">Show type for filtering (Series, Miniseries, Documentary, etc.)</param>
        /// <returns>Collection of shows matching the specified type classification</returns>
        Task<IEnumerable<TvShowDto>> GetTvShowsByTypeAsync(string type);

        /// <summary>
        /// Retrieves all episodes associated with a specific TV show.
        /// Provides comprehensive episode collection organized for optimal viewing experience.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show whose episodes to retrieve</param>
        /// <returns>Collection of episodes with complete metadata, ordered by season and episode number</returns>
        Task<IEnumerable<EpisodeDto>> GetTvShowEpisodesAsync(int tvShowId);

        /// <summary>
        /// Retrieves all actors and cast members associated with a specific TV show.
        /// Provides comprehensive cast information including character details and actor metadata.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show whose cast to retrieve</param>
        /// <returns>Collection of actors with character information and professional details</returns>
        Task<IEnumerable<ActorDto>> GetTvShowActorsAsync(int tvShowId);
    }
}