using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing TV show operations and providing comprehensive show data.
    /// Handles CRUD operations, search functionality, filtering, and related entity retrieval
    /// for TV shows within the system.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Base authorization requirement for all endpoints
    [Produces("application/json")]
    public class TvShowsController : ControllerBase
    {
        private readonly ITvShowService _tvShowService;
        private readonly ILogger<TvShowsController> _logger;

        /// <summary>
        /// Initializes a new instance of the TvShowsController.
        /// </summary>
        /// <param name="tvShowService">Service for TV show-related operations</param>
        /// <param name="logger">Logger instance for tracking operations and errors</param>
        public TvShowsController(ITvShowService tvShowService, ILogger<TvShowsController> logger)
        {
            _tvShowService = tvShowService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all TV shows with comprehensive pagination, filtering, and sorting capabilities.
        /// </summary>
        /// <param name="parameters">Query parameters for pagination, filtering, and sorting</param>
        /// <returns>Paged result containing TV show DTOs and pagination metadata</returns>
        /// <response code="200">TV shows retrieved successfully with pagination info</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TvShowDto>>> GetTvShows([FromQuery] QueryParameters parameters)
        {
            try
            {
                // Delegate to service layer which handles all filtering, pagination, and sorting logic
                var result = await _tvShowService.GetAllTvShowsAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log error without exposing query details that might contain sensitive information
                _logger.LogError(ex, "Error retrieving TV shows");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves detailed information for a specific TV show by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the TV show</param>
        /// <returns>Detailed TV show information including episodes, actors, and user-specific data</returns>
        /// <response code="200">TV show details retrieved successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="404">TV show not found</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TvShowDetailDto>> GetTvShow(int id)
        {
            try
            {
                // Extract user ID for personalized data if user is authenticated
                int? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst("userId")?.Value;
                    // Safely parse user ID claim with validation
                    if (int.TryParse(userIdClaim, out var parsedUserId))
                        userId = parsedUserId;
                }

                // Retrieve detailed show information with optional user context
                var tvShow = await _tvShowService.GetTvShowByIdAsync(id, userId);

                // Handle case where show doesn't exist
                if (tvShow == null)
                    return NotFound($"TV show with ID {id} not found");

                return Ok(tvShow);
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error retrieving TV show {TvShowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a new TV show entry in the system.
        /// </summary>
        /// <param name="createDto">TV show creation data including all required and optional fields</param>
        /// <returns>Created TV show with generated ID and timestamps</returns>
        /// <response code="201">TV show created successfully</response>
        /// <response code="400">Invalid input data or validation errors</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during creation process</response>
        [HttpPost]
        [Authorize] // Explicit authorization required for content creation
        public async Task<ActionResult<TvShowDto>> CreateTvShow([FromBody] CreateTvShowDto createDto)
        {
            try
            {
                // Validate input data against model requirements
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Create new show through service layer
                var tvShow = await _tvShowService.CreateTvShowAsync(createDto);

                // Return 201 Created with location header pointing to the new resource
                return CreatedAtAction(nameof(GetTvShow), new { id = tvShow.Id }, tvShow);
            }
            catch (Exception ex)
            {
                // Log creation error without exposing input data in logs
                _logger.LogError(ex, "Error creating TV show");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing TV show with new information.
        /// </summary>
        /// <param name="id">The unique identifier of the TV show to update</param>
        /// <param name="updateDto">Updated TV show data</param>
        /// <returns>Updated TV show information</returns>
        /// <response code="200">TV show updated successfully</response>
        /// <response code="400">Invalid input data or validation errors</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="404">TV show not found</response>
        /// <response code="500">Internal server error during update process</response>
        [HttpPut("{id:int}")]
        [Authorize] // Explicit authorization required for content modification
        public async Task<ActionResult<TvShowDto>> UpdateTvShow(int id, [FromBody] CreateTvShowDto updateDto)
        {
            try
            {
                // Validate input data against model requirements
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Attempt to update the show through service layer
                var tvShow = await _tvShowService.UpdateTvShowAsync(id, updateDto);

                // Handle case where show doesn't exist
                if (tvShow == null)
                    return NotFound($"TV show with ID {id} not found");

                return Ok(tvShow);
            }
            catch (Exception ex)
            {
                // Log update error with show ID for debugging
                _logger.LogError(ex, "Error updating TV show {TvShowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a TV show and all associated data from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the TV show to delete</param>
        /// <returns>No content if deletion was successful</returns>
        /// <response code="204">TV show deleted successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="404">TV show not found</response>
        /// <response code="500">Internal server error during deletion process</response>
        [HttpDelete("{id:int}")]
        [Authorize] // Explicit authorization required for content deletion
        public async Task<IActionResult> DeleteTvShow(int id)
        {
            try
            {
                // Attempt to delete the show through service layer
                var success = await _tvShowService.DeleteTvShowAsync(id);

                // Handle case where show doesn't exist
                if (!success)
                    return NotFound($"TV show with ID {id} not found");

                // Return 204 No Content for successful deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log deletion error with show ID for debugging
                _logger.LogError(ex, "Error deleting TV show {TvShowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Searches for TV shows using text-based query across multiple fields.
        /// </summary>
        /// <param name="query">Search query string to match against show titles and descriptions</param>
        /// <returns>Collection of TV shows matching the search criteria</returns>
        /// <response code="200">Search completed successfully</response>
        /// <response code="400">Empty or invalid search query</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during search process</response>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> SearchTvShows([FromQuery] string query)
        {
            try
            {
                // Validate that search query is provided and not empty
                if (string.IsNullOrEmpty(query))
                    return BadRequest("Search query cannot be empty");

                // Perform search through service layer with optimized indexing
                var tvShows = await _tvShowService.SearchTvShowsAsync(query);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                // Log search error with query for debugging (consider privacy implications)
                _logger.LogError(ex, "Error searching TV shows with query: {Query}", query);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves TV shows filtered by a specific genre classification.
        /// </summary>
        /// <param name="genre">Genre name to filter shows (e.g., "Drama", "Comedy", "Action")</param>
        /// <returns>Collection of TV shows belonging to the specified genre</returns>
        /// <response code="200">Shows retrieved successfully for the specified genre</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetTvShowsByGenre(string genre)
        {
            try
            {
                // Retrieve shows filtered by the specified genre
                var tvShows = await _tvShowService.GetTvShowsByGenreAsync(genre);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                // Log error with genre for debugging purposes
                _logger.LogError(ex, "Error retrieving TV shows for genre: {Genre}", genre);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves TV shows filtered by show type classification.
        /// </summary>
        /// <param name="type">Show type to filter by (e.g., "Series", "Miniseries", "Documentary")</param>
        /// <returns>Collection of TV shows of the specified type</returns>
        /// <response code="200">Shows retrieved successfully for the specified type</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetTvShowsByType(string type)
        {
            try
            {
                // Retrieve shows filtered by the specified type
                var tvShows = await _tvShowService.GetTvShowsByTypeAsync(type);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                // Log error with type for debugging purposes
                _logger.LogError(ex, "Error retrieving TV shows for type: {Type}", type);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves all episodes for a specific TV show organized by seasons.
        /// </summary>
        /// <param name="id">The unique identifier of the TV show</param>
        /// <returns>Collection of episodes with season and episode number information</returns>
        /// <response code="200">Episodes retrieved successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet("{id:int}/episodes")]
        public async Task<ActionResult<IEnumerable<EpisodeDto>>> GetTvShowEpisodes(int id)
        {
            try
            {
                // Retrieve all episodes for the specified TV show
                var episodes = await _tvShowService.GetTvShowEpisodesAsync(id);
                return Ok(episodes);
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error retrieving episodes for TV show {TvShowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves all actors and cast members associated with a specific TV show.
        /// </summary>
        /// <param name="id">The unique identifier of the TV show</param>
        /// <returns>Collection of actors with their role information and character details</returns>
        /// <response code="200">Actors retrieved successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during retrieval process</response>
        [HttpGet("{id:int}/actors")]
        public async Task<ActionResult<IEnumerable<ActorDto>>> GetTvShowActors(int id)
        {
            try
            {
                // Retrieve all actors associated with the specified TV show
                var actors = await _tvShowService.GetTvShowActorsAsync(id);
                return Ok(actors);
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error retrieving actors for TV show {TvShowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}