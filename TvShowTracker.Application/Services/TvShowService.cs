using AutoMapper;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;

namespace TvShowTracker.Application.Services
{
    /// <summary>
    /// Core service implementation for comprehensive TV show management and data operations.
    /// Serves as the primary business logic layer orchestrating complex show operations, data validation,
    /// and user context integration throughout the application.
    /// </summary>
    public class TvShowService : ITvShowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes the TvShowService with required dependencies for data access and object mapping.
        /// Establishes the foundation for complex business operations and data orchestration.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work instance providing coordinated data access across repositories</param>
        /// <param name="mapper">AutoMapper instance for efficient entity-to-DTO conversions</param>
        public TvShowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paginated collection of TV shows with comprehensive filtering, sorting, and search capabilities.
        /// Implements robust parameter validation and efficient query delegation for optimal performance.
        /// </summary>
        /// <param name="parameters">Query parameters containing pagination, filtering, sorting, and search criteria</param>
        /// <returns>Paginated result with show data and comprehensive pagination metadata</returns>
        public async Task<PagedResultDto<TvShowDto>> GetAllTvShowsAsync(QueryParameters parameters)
        {
            // Validate and sanitize parameters to ensure system security and stability
            parameters.Page = Math.Max(1, parameters.Page);                    // Ensure minimum page value
            parameters.PageSize = Math.Clamp(parameters.PageSize, 1, 50);      // Prevent resource exhaustion

            // Delegate to repository for optimized database operations
            var (tvShows, totalCount) = await _unitOfWork.TvShows.GetPagedAsync(
                parameters.Page,
                parameters.PageSize,
                parameters.SortBy,
                parameters.SortDescending,
                parameters.Genre,
                parameters.Type,
                parameters.Search);

            // Convert entities to DTOs using AutoMapper for consistent object mapping
            var tvShowDtos = _mapper.Map<IEnumerable<TvShowDto>>(tvShows);

            // Create paginated result with comprehensive metadata
            return new PagedResultDto<TvShowDto>(tvShowDtos, parameters.Page, parameters.PageSize, totalCount);
        }

        /// <summary>
        /// Retrieves comprehensive details for a specific TV show with optional user context integration.
        /// Orchestrates multiple data sources to provide complete show information with personalization.
        /// </summary>
        /// <param name="id">Unique identifier of the TV show to retrieve</param>
        /// <param name="userId">Optional user identifier for personalized data inclusion</param>
        /// <returns>Detailed show information with related entities and user context, or null if not found</returns>
        public async Task<TvShowDetailDto?> GetTvShowByIdAsync(int id, int? userId = null)
        {
            // Load show with episodes using optimized repository method
            var tvShow = await _unitOfWork.TvShows.GetByIdWithEpisodesAsync(id);
            if (tvShow == null)
                return null; // Show not found

            // Load actors separately to optimize query performance and prevent cartesian products
            var tvShowWithActors = await _unitOfWork.TvShows.GetByIdWithActorsAsync(id);

            // Map core show data to detailed DTO using AutoMapper
            var detailDto = _mapper.Map<TvShowDetailDto>(tvShow);

            // Integrate actor information if available
            if (tvShowWithActors?.Actors != null)
            {
                detailDto.Actors = _mapper.Map<List<ActorDto>>(tvShowWithActors.Actors);
            }

            // Add user-specific context when user ID is provided
            if (userId.HasValue)
            {
                detailDto.IsFavorite = await _unitOfWork.UserFavorites.IsFavoriteAsync(userId.Value, id);
            }

            return detailDto;
        }

        /// <summary>
        /// Creates a new TV show entry with comprehensive validation and audit trail establishment.
        /// Implements proper business logic for show creation with system timestamp management.
        /// </summary>
        /// <param name="createDto">Complete show creation data with metadata and classification</param>
        /// <returns>Created show data with system-generated identifiers and timestamps</returns>
        public async Task<TvShowDto> CreateTvShowAsync(CreateTvShowDto createDto)
        {
            // Map DTO to entity using AutoMapper for consistent data transformation
            var tvShow = _mapper.Map<TvShow>(createDto);

            // Set system-managed timestamps for audit trail and change tracking
            tvShow.CreatedAt = DateTime.UtcNow;
            tvShow.UpdatedAt = DateTime.UtcNow;

            // Create entity through repository layer with proper validation
            var createdTvShow = await _unitOfWork.TvShows.AddAsync(tvShow);
            await _unitOfWork.SaveChangesAsync();

            // Return mapped DTO with system-generated data
            return _mapper.Map<TvShowDto>(createdTvShow);
        }

        /// <summary>
        /// Updates an existing TV show with new information while maintaining data integrity and audit trails.
        /// Implements comprehensive validation and change tracking for reliable content management.
        /// </summary>
        /// <param name="id">Unique identifier of the show to update</param>
        /// <param name="updateDto">Updated show data with new metadata and classification</param>
        /// <returns>Updated show data reflecting changes, or null if show not found</returns>
        public async Task<TvShowDto?> UpdateTvShowAsync(int id, CreateTvShowDto updateDto)
        {
            // Validate show existence before attempting update operations
            var existingTvShow = await _unitOfWork.TvShows.GetByIdAsync(id);
            if (existingTvShow == null)
                return null; // Show not found

            // Map updated data to existing entity, preserving unmapped properties
            _mapper.Map(updateDto, existingTvShow);

            // Update system timestamp for audit trail and change tracking
            existingTvShow.UpdatedAt = DateTime.UtcNow;

            // Persist changes through repository layer with validation
            await _unitOfWork.TvShows.UpdateAsync(existingTvShow);
            await _unitOfWork.SaveChangesAsync();

            // Return updated entity as DTO
            return _mapper.Map<TvShowDto>(existingTvShow);
        }

        /// <summary>
        /// Removes a TV show and all associated data with proper validation and cascade handling.
        /// Implements safe deletion with existence validation and transactional consistency.
        /// </summary>
        /// <param name="id">Unique identifier of the show to delete</param>
        /// <returns>True if deletion was successful, false if show was not found</returns>
        public async Task<bool> DeleteTvShowAsync(int id)
        {
            // Validate show existence before attempting deletion
            var tvShow = await _unitOfWork.TvShows.GetByIdAsync(id);
            if (tvShow == null)
                return false; // Show not found

            // Delete entity through repository layer with cascade handling
            await _unitOfWork.TvShows.DeleteAsync(tvShow);
            await _unitOfWork.SaveChangesAsync();

            return true; // Deletion successful
        }

        /// <summary>
        /// Performs comprehensive text-based search across TV show content with optimized query delegation.
        /// Provides intelligent search results through repository-level search implementation.
        /// </summary>
        /// <param name="searchTerm">Text query for searching show names and related content</param>
        /// <returns>Collection of shows matching search criteria with relevance-based ordering</returns>
        public async Task<IEnumerable<TvShowDto>> SearchTvShowsAsync(string searchTerm)
        {
            // Delegate to repository for optimized search implementation
            var tvShows = await _unitOfWork.TvShows.SearchByNameAsync(searchTerm);

            // Convert search results to DTOs using AutoMapper
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        /// <summary>
        /// Retrieves TV shows filtered by specific genre with efficient query delegation.
        /// Provides genre-based content discovery through optimized repository methods.
        /// </summary>
        /// <param name="genre">Genre name for filtering shows</param>
        /// <returns>Collection of shows belonging to the specified genre</returns>
        public async Task<IEnumerable<TvShowDto>> GetTvShowsByGenreAsync(string genre)
        {
            // Delegate to repository for optimized genre-based filtering
            var tvShows = await _unitOfWork.TvShows.GetByGenreAsync(genre);

            // Convert filtered results to DTOs
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        /// <summary>
        /// Retrieves TV shows filtered by content type with efficient repository delegation.
        /// Enables format-based content discovery through optimized data access patterns.
        /// </summary>
        /// <param name="type">Show type for filtering content</param>
        /// <returns>Collection of shows matching the specified type classification</returns>
        public async Task<IEnumerable<TvShowDto>> GetTvShowsByTypeAsync(string type)
        {
            // Delegate to repository for optimized type-based filtering
            var tvShows = await _unitOfWork.TvShows.GetByTypeAsync(type);

            // Convert filtered results to DTOs
            return _mapper.Map<IEnumerable<TvShowDto>>(tvShows);
        }

        /// <summary>
        /// Retrieves all episodes for a specific TV show with efficient repository delegation.
        /// Provides comprehensive episode collection through optimized data access.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show whose episodes to retrieve</param>
        /// <returns>Collection of episodes with complete metadata ordered by season and episode</returns>
        public async Task<IEnumerable<EpisodeDto>> GetTvShowEpisodesAsync(int tvShowId)
        {
            // Delegate to episode repository for specialized episode retrieval
            var episodes = await _unitOfWork.Episodes.GetByTvShowIdAsync(tvShowId);

            // Convert episodes to DTOs with show context
            return _mapper.Map<IEnumerable<EpisodeDto>>(episodes);
        }

        /// <summary>
        /// Retrieves all actors and cast members for a specific TV show with safe null handling.
        /// Provides comprehensive cast information through optimized relationship loading.
        /// </summary>
        /// <param name="tvShowId">Unique identifier of the TV show whose cast to retrieve</param>
        /// <returns>Collection of actors with character information and professional details</returns>
        public async Task<IEnumerable<ActorDto>> GetTvShowActorsAsync(int tvShowId)
        {
            // Load show with actor relationships using specialized repository method
            var tvShow = await _unitOfWork.TvShows.GetByIdWithActorsAsync(tvShowId);

            // Convert actors to DTOs with safe null handling
            return _mapper.Map<IEnumerable<ActorDto>>(tvShow?.Actors ?? new List<Actor>());
        }
    }
}