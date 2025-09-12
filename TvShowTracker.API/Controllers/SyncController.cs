using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvShowTracker.Infrastructure.Data;
using TvShowTracker.Infrastructure.ExternalServices;

namespace TvShowTracker.API.Controllers
{
    /// <summary>
    /// Controller responsible for data synchronization operations and system statistics.
    /// Provides endpoints for monitoring database state, testing external API connectivity,
    /// and performing manual synchronization operations for development purposes.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all administrative operations
    [Produces("application/json")]
    public class SyncController : ControllerBase
    {
        private readonly TvShowContext _context;
        private readonly IExternalTvShowService _externalService;
        private readonly ILogger<SyncController> _logger;

        /// <summary>
        /// Initializes a new instance of the SyncController.
        /// </summary>
        /// <param name="context">Database context for direct data access</param>
        /// <param name="externalService">Service for interacting with external TV show APIs</param>
        /// <param name="logger">Logger instance for tracking operations and errors</param>
        public SyncController(
            TvShowContext context,
            IExternalTvShowService externalService,
            ILogger<SyncController> logger)
        {
            _context = context;
            _externalService = externalService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves comprehensive database statistics and system health information.
        /// </summary>
        /// <returns>Object containing various database metrics and synchronization status</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during statistics calculation</response>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetSyncStats()
        {
            try
            {
                // Aggregate multiple database queries to provide comprehensive statistics
                var stats = new
                {
                    // Core entity counts - provides overview of database population
                    TotalShows = await _context.TvShows.CountAsync(),
                    TotalEpisodes = await _context.Episodes.CountAsync(),
                    TotalActors = await _context.Actors.CountAsync(),
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalFavorites = await _context.UserFavorites.CountAsync(),

                    // Last sync time - indicates freshness of external data
                    // Uses the most recently updated show as a proxy for last sync time
                    LastSyncTime = await _context.TvShows
                        .OrderByDescending(ts => ts.UpdatedAt)
                        .Select(ts => ts.UpdatedAt)
                        .FirstOrDefaultAsync(),

                    // Recent activity metric - shows added in the last 7 days
                    // Useful for monitoring synchronization frequency and system activity
                    RecentlyAdded = await _context.TvShows
                        .Where(ts => ts.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                        .CountAsync()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                // Log error without exposing internal database structure
                _logger.LogError(ex, "Error retrieving database statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Performs manual synchronization with external TV show API for development and testing purposes.
        /// </summary>
        /// <returns>Object containing synchronization results and number of processed shows</returns>
        /// <response code="200">Manual synchronization completed successfully</response>
        /// <response code="400">No shows retrieved from external API</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during synchronization process</response>
        [HttpPost("force-sync")]
        public async Task<IActionResult> ForceSync()
        {
            try
            {
                // Log the initiation of manual synchronization for auditing
                _logger.LogInformation("Manual synchronization initiated");

                // Fetch limited data from external API (1 page for performance)
                var externalShows = await _externalService.GetAllPopularShowsAsync(maxPages: 1);

                // Validate that external service returned data
                if (!externalShows.Any())
                {
                    return BadRequest("No shows retrieved from external API");
                }

                int processed = 0;

                // Process a limited number of shows for manual testing (5 shows max)
                foreach (var externalShow in externalShows.Take(5))
                {
                    // Check if show already exists in database (case-insensitive name comparison)
                    var existingShow = await _context.TvShows
                        .FirstOrDefaultAsync(ts => ts.Name.ToLower() == externalShow.Name.ToLower());

                    // Only add shows that don't already exist
                    if (existingShow == null)
                    {
                        // Create new TV show entity from external data
                        var newShow = new Core.Entities.TvShow
                        {
                            Name = externalShow.Name,
                            Description = "Synchronized from external API", // Default description for external shows
                            Network = externalShow.Network,
                            Status = externalShow.Status,
                            ImageUrl = externalShow.ImageThumbnailPath,
                            ShowType = "Series", // Default show type
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Genres = new List<string> { "General" } // Default genre assignment
                        };

                        // Safely parse start date from external source
                        if (DateTime.TryParse(externalShow.StartDate, out var startDate))
                        {
                            newShow.StartDate = startDate;
                        }
                        else
                        {
                            // Fallback to current date if parsing fails
                            newShow.StartDate = DateTime.UtcNow;
                        }

                        // Add to context for batch insertion
                        _context.TvShows.Add(newShow);
                        processed++;
                    }
                }

                // Commit all changes to database in a single transaction
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Manual synchronization completed",
                    processedShows = processed
                });
            }
            catch (Exception ex)
            {
                // Log detailed error information for debugging
                _logger.LogError(ex, "Error during manual synchronization");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Tests connectivity to the external TV show API and validates service availability.
        /// </summary>
        /// <returns>Object containing connection status and basic API response information</returns>
        /// <response code="200">External API connection successful</response>
        /// <response code="400">Unable to connect to external API</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="500">Internal server error during connection test</response>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestExternalConnection()
        {
            try
            {
                // Perform minimal API call to test connectivity (page 1 only)
                var response = await _externalService.GetPopularShowsAsync(1);

                // Validate that the external service returned a valid response
                if (response == null)
                {
                    return BadRequest("Unable to connect to external API");
                }

                // Return connection status with basic API response metadata
                return Ok(new
                {
                    message = "External API connection successful",
                    totalShows = response.Total,
                    currentPage = response.Page,
                    totalPages = response.Pages,
                    showsInPage = response.TvShows?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                // Log connection error details for debugging
                _logger.LogError(ex, "Error testing external API connection");

                // Return the exception message for debugging purposes
                // Note: In production, consider returning a generic error message
                return StatusCode(500, ex.Message);
            }
        }
    }
}