using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user favorite TV shows.
    /// Handles all operations related to favorite show management including adding, removing, 
    /// checking status, and retrieving user's favorite shows collection.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    [Produces("application/json")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoritesController> _logger;

        /// <summary>
        /// Initializes a new instance of the FavoritesController.
        /// </summary>
        /// <param name="favoriteService">Service for favorite-related operations</param>
        /// <param name="logger">Logger instance for tracking operations and errors</param>
        public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
        {
            _favoriteService = favoriteService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all favorite TV shows for the authenticated user.
        /// </summary>
        /// <returns>Collection of TV show DTOs representing user's favorites</returns>
        /// <response code="200">Favorites successfully retrieved</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during favorites retrieval</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetUserFavorites()
        {
            try
            {
                // Extract user ID from JWT token for security and context
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Retrieve all favorite shows for the authenticated user
                var favorites = await _favoriteService.GetUserFavoritesAsync(userId.Value);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                // Log error without exposing sensitive user information
                _logger.LogError(ex, "Error retrieving user favorites");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Adds a TV show to the authenticated user's favorites list.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show to add to favorites</param>
        /// <returns>Success message if the show was added successfully</returns>
        /// <response code="200">Show successfully added to favorites</response>
        /// <response code="400">Show not found or already in favorites</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during addition process</response>
        [HttpPost("{tvShowId:int}")]
        public async Task<IActionResult> AddFavorite(int tvShowId)
        {
            try
            {
                // Validate user authentication
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Attempt to add the show to user's favorites
                var success = await _favoriteService.AddFavoriteAsync(userId.Value, tvShowId);

                // Handle business logic failures (show not found, already favorite, etc.)
                if (!success)
                    return BadRequest("Unable to add to favorites. Show not found or already favorited.");

                return Ok(new { message = "Show added to favorites successfully" });
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error adding show {TvShowId} to favorites", tvShowId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Removes a TV show from the authenticated user's favorites list.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show to remove from favorites</param>
        /// <returns>Success message if the show was removed successfully</returns>
        /// <response code="200">Show successfully removed from favorites</response>
        /// <response code="400">Show is not in the user's favorites</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during removal process</response>
        [HttpDelete("{tvShowId:int}")]
        public async Task<IActionResult> RemoveFavorite(int tvShowId)
        {
            try
            {
                // Validate user authentication
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Attempt to remove the show from user's favorites
                var success = await _favoriteService.RemoveFavoriteAsync(userId.Value, tvShowId);

                // Handle case where show is not in favorites
                if (!success)
                    return BadRequest("Unable to remove from favorites. Show is not in favorites.");

                return Ok(new { message = "Show removed from favorites successfully" });
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error removing show {TvShowId} from favorites", tvShowId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Checks if a specific TV show is marked as favorite by the authenticated user.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show to check</param>
        /// <returns>Object containing boolean flag indicating favorite status</returns>
        /// <response code="200">Check completed successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during check process</response>
        [HttpGet("check/{tvShowId:int}")]
        public async Task<ActionResult<object>> IsFavorite(int tvShowId)
        {
            try
            {
                // Validate user authentication
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Check if the show is in user's favorites
                var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tvShowId);
                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error checking if show {TvShowId} is favorite", tvShowId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves the total count of favorite shows for the authenticated user.
        /// </summary>
        /// <returns>Object containing the count of user's favorite shows</returns>
        /// <response code="200">Count retrieved successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during count retrieval</response>
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetFavoritesCount()
        {
            try
            {
                // Validate user authentication
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Get the total count of user's favorites
                var count = await _favoriteService.GetFavoritesCountAsync(userId.Value);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                // Log error without exposing user-specific information
                _logger.LogError(ex, "Error retrieving favorites count");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Toggles the favorite status of a TV show for the authenticated user.
        /// If the show is currently a favorite, it will be removed. If not, it will be added.
        /// </summary>
        /// <param name="tvShowId">The ID of the TV show to toggle</param>
        /// <returns>Object containing success message and new favorite status</returns>
        /// <response code="200">Toggle operation completed successfully</response>
        /// <response code="400">Unable to perform toggle operation</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during toggle process</response>
        [HttpPost("toggle/{tvShowId:int}")]
        public async Task<ActionResult<object>> ToggleFavorite(int tvShowId)
        {
            try
            {
                // Validate user authentication
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                // Check current favorite status to determine action
                var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tvShowId);

                bool success;
                string message;

                // Perform opposite operation based on current status
                if (isFavorite)
                {
                    // Remove from favorites if currently favorited
                    success = await _favoriteService.RemoveFavoriteAsync(userId.Value, tvShowId);
                    message = "Show removed from favorites";
                }
                else
                {
                    // Add to favorites if not currently favorited
                    success = await _favoriteService.AddFavoriteAsync(userId.Value, tvShowId);
                    message = "Show added to favorites";
                }

                // Handle operation failure
                if (!success)
                    return BadRequest("Unable to toggle favorite status");

                // Return success response with new status
                return Ok(new
                {
                    message,
                    isFavorite = !isFavorite // Return the new status
                });
            }
            catch (Exception ex)
            {
                // Log error with show ID for debugging purposes
                _logger.LogError(ex, "Error toggling favorite status for show {TvShowId}", tvShowId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Helper method to extract the current user's ID from JWT token claims.
        /// This method centralizes user ID extraction logic and provides type safety.
        /// </summary>
        /// <returns>User ID if valid token is present, null otherwise</returns>
        private int? GetCurrentUserId()
        {
            // Extract userId claim from JWT token
            var userIdClaim = User.FindFirst("userId")?.Value;

            // Attempt to parse the claim value as integer
            // Return parsed value if successful, null if parsing fails
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}