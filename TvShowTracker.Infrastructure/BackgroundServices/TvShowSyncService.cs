using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TvShowTracker.Core.Entities;
using TvShowTracker.Infrastructure.Data;
using TvShowTracker.Infrastructure.ExternalServices;
using TvShowTracker.Infrastructure.ExternalServices.Models;

namespace TvShowTracker.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background service responsible for syncing TV shows from external APIs.
    /// Runs periodically to keep the database updated with information on popular shows.
    /// </summary>
    public class TvShowSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TvShowSyncService> _logger;
        private readonly TimeSpan _syncInterval;

        /// <summary>
        /// Initializes a new instance of the TV show synchronization service.
        /// </summary>
        /// <param name="serviceProvider">Service provider for creating scopes during execution.</param>
        /// <param name="logger">Logger to record events and errors.</param>
        /// <param name="configuration">Application configuration to retrieve synchronization parameters.</param>
        public TvShowSyncService(
            IServiceProvider serviceProvider,
            ILogger<TvShowSyncService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Configure sync interval (default: 6 hours)
            var intervalHours = configuration.GetSection("BackgroundSync")["IntervalHours"];
            var hours = int.TryParse(intervalHours, out var parsedHours) ? parsedHours : 6;
            _syncInterval = TimeSpan.FromHours(hours);

            _logger.LogInformation("TvShowSyncService configured with an interval of {Hours} hours", hours);
        }

        /// <summary>
        /// Executes the sync process in a continuous loop at the configured interval.
        /// Waits 30 seconds before the first execution and then repeats periodically.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token to stop the service gracefully.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TvShowSyncService started");

            // Initial sync after 30 seconds delay
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncTvShowsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during TV show synchronization");
                }

                try
                {
                    await Task.Delay(_syncInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Service is stopping
                    break;
                }
            }

            _logger.LogInformation("TvShowSyncService stopped");
        }

        /// <summary>
        /// Performs a complete synchronization operation for TV shows.
        /// Retrieves popular shows from the external API, processes each one, and updates the database.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to interrupt the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SyncTvShowsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting TV show synchronization...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();
            var externalService = scope.ServiceProvider.GetRequiredService<IExternalTvShowService>();

            try
            {
                // Retrieve popular shows from the external API
                var externalShows = await externalService.GetAllPopularShowsAsync(maxPages: 3);

                if (!externalShows.Any())
                {
                    _logger.LogWarning("No TV shows retrieved from external API");
                    return;
                }

                int newShows = 0;
                int updatedShows = 0;
                int processedShows = 0;

                foreach (var externalShow in externalShows)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        var result = await ProcessTvShowAsync(context, externalService, externalShow);
                        newShows += result.NewShows;
                        updatedShows += result.UpdatedShows;
                        processedShows++;

                        // Delay every 5 processed shows to avoid API overloading
                        if (processedShows % 5 == 0)
                        {
                            await Task.Delay(2000, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing show {ShowName} (ID: {ShowId})",
                            externalShow.Name, externalShow.Id);
                    }
                }

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Synchronization completed: {New} new shows, {Updated} updated, {Total} processed",
                    newShows, updatedShows, processedShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during synchronization");
            }
        }

        /// <summary>
        /// Processes an individual show from the external API, determining whether to create or update it.
        /// Checks if the show already exists in the database by name and performs the appropriate action.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="externalService">Service to access the external API.</param>
        /// <param name="externalShow">TV show data from the external API.</param>
        /// <returns>A result indicating how many shows were created or updated.</returns>
        private async Task<ProcessResult> ProcessTvShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            EpisodateTvShow externalShow)
        {
            var result = new ProcessResult();

            // Check if the show already exists (by name, since external IDs may not match)
            var existingShow = await context.TvShows
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == externalShow.Name.ToLower());

            if (existingShow != null)
            {
                // Update existing show if necessary
                if (ShouldUpdateShow(existingShow, externalShow))
                {
                    await UpdateExistingShowAsync(context, externalService, existingShow, externalShow);
                    result.UpdatedShows = 1;
                }
                return result;
            }

            // Create new show
            await CreateNewShowAsync(context, externalService, externalShow);
            result.NewShows = 1;
            return result;
        }

        /// <summary>
        /// Determines whether an existing show should be updated based on its last modified date.
        /// </summary>
        /// <param name="existingShow">Existing show in the database.</param>
        /// <param name="externalShow">TV show data from the external API.</param>
        /// <returns>True if the show should be updated; otherwise, false.</returns>
        private bool ShouldUpdateShow(TvShow existingShow, EpisodateTvShow externalShow)
        {
            // Update if the last update was more than 1 day ago
            return DateTime.UtcNow - existingShow.UpdatedAt > TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Updates an existing show with the latest information from the external API.
        /// Retrieves updated details and updates any changed fields.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="externalService">Service to access the external API.</param>
        /// <param name="existingShow">The existing show to update.</param>
        /// <param name="externalShow">Updated TV show data from the external API.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateExistingShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            TvShow existingShow,
            EpisodateTvShow externalShow)
        {
            try
            {
                // Fetch updated details
                var showDetails = await externalService.GetShowDetailsAsync(externalShow.Id);
                if (showDetails?.TvShow != null)
                {
                    var details = showDetails.TvShow;

                    // Update fields
                    existingShow.Status = details.Status;
                    existingShow.Network = details.Network;
                    existingShow.UpdatedAt = DateTime.UtcNow;

                    if (double.TryParse(details.Rating, out var rating))
                    {
                        existingShow.Rating = rating;
                    }

                    _logger.LogDebug("Show updated: {ShowName}", existingShow.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating show {ShowName}", existingShow.Name);
            }
        }

        /// <summary>
        /// Creates a new show in the database using data from the external API.
        /// Fetches full details of the show and maps the data to the local entity.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="externalService">Service to access the external API.</param>
        /// <param name="externalShow">TV show data from the external API.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task CreateNewShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            EpisodateTvShow externalShow)
        {
            try
            {
                // Fetch full show details
                var showDetails = await externalService.GetShowDetailsAsync(externalShow.Id);

                var tvShow = new TvShow
                {
                    Name = externalShow.Name,
                    Description = showDetails?.TvShow?.Description ?? "Description not available",
                    Network = externalShow.Network,
                    Status = externalShow.Status,
                    ImageUrl = externalShow.ImageThumbnailPath,
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = showDetails?.TvShow?.Genres ?? new List<string> { "Unknown" }
                };

                // Parse start date
                if (DateTime.TryParse(externalShow.StartDate, out var startDate))
                {
                    tvShow.StartDate = startDate;
                }
                else
                {
                    tvShow.StartDate = DateTime.UtcNow;
                }

                // Parse rating
                if (double.TryParse(showDetails?.TvShow?.Rating, out var rating))
                {
                    tvShow.Rating = Math.Max(0, Math.Min(10, rating));
                }

                context.TvShows.Add(tvShow);

                _logger.LogDebug("New show created: {ShowName}", tvShow.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new show {ShowName}", externalShow.Name);
            }
        }

        /// <summary>
        /// Represents the processing result of a show, indicating how many were created or updated.
        /// </summary>
        private class ProcessResult
        {
            /// <summary>
            /// Number of newly created shows.
            /// </summary>
            public int NewShows { get; set; } = 0;

            /// <summary>
            /// Number of existing shows that were updated.
            /// </summary>
            public int UpdatedShows { get; set; } = 0;
        }
    }
}
