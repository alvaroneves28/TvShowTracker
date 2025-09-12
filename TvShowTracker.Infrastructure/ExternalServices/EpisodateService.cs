using Microsoft.Extensions.Logging;
using System.Text.Json;
using TvShowTracker.Infrastructure.ExternalServices.Models;

namespace TvShowTracker.Infrastructure.ExternalServices
{
    /// <summary>
    /// Service to interact with the Episodate API.
    /// Provides methods to fetch popular TV shows and detailed information about a specific show.
    /// </summary>
    public class EpisodateService : IExternalTvShowService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EpisodateService> _logger;
        private const string BaseUrl = "https://www.episodate.com/api";

        /// <summary>
        /// Initializes a new instance of the <see cref="EpisodateService"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to make API requests.</param>
        /// <param name="logger">The logger instance.</param>
        public EpisodateService(HttpClient httpClient, ILogger<EpisodateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configure default headers
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TvShowTracker/1.0");
        }

        /// <summary>
        /// Retrieves a page of the most popular TV shows from the Episodate API.
        /// </summary>
        /// <param name="page">The page number to retrieve. Default is 1.</param>
        /// <returns>An <see cref="EpisodateResponse"/> containing the list of TV shows, or null if an error occurs.</returns>
        public async Task<EpisodateResponse?> GetPopularShowsAsync(int page = 1)
        {
            try
            {
                var url = $"{BaseUrl}/most-popular?page={page}";
                _logger.LogInformation("Requesting popular shows from URL: {Url}", url);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("Cache-Control", "no-cache");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("HTTP error {StatusCode}: {Content}", response.StatusCode, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(jsonContent))
                {
                    _logger.LogError("Response content is empty");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<EpisodateResponse>(jsonContent, options);
                _logger.LogInformation("Deserialization completed. Total shows: {Count}", result?.TvShows?.Count ?? 0);

                return result;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request error on page {Page}: {Message}", page, httpEx.Message);
                return null;
            }
            catch (TaskCanceledException timeoutEx)
            {
                _logger.LogError(timeoutEx, "Timeout when fetching shows from page {Page}", page);
                return null;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error on page {Page}: {Message}", page, jsonEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error fetching shows from page {Page}: {Message}", page, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific TV show from the Episodate API.
        /// </summary>
        /// <param name="showId">The ID of the TV show.</param>
        /// <returns>An <see cref="EpisodateTvShowDetail"/> with detailed information, or null if an error occurs.</returns>
        public async Task<EpisodateTvShowDetail?> GetShowDetailsAsync(int showId)
        {
            try
            {
                var url = $"{BaseUrl}/show-details?q={showId}";
                _logger.LogInformation("Fetching details for show {ShowId}", showId);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error fetching show details {ShowId}: {StatusCode}", showId, response.StatusCode);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<EpisodateTvShowDetail>(jsonContent, options);

                _logger.LogInformation("Show details for {ShowId} obtained successfully", showId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching details for show {ShowId}", showId);
                return null;
            }
        }

        /// <summary>
        /// Retrieves multiple pages of popular TV shows from the Episodate API, up to a maximum number of pages.
        /// </summary>
        /// <param name="maxPages">The maximum number of pages to retrieve. Default is 5.</param>
        /// <returns>A list of <see cref="EpisodateTvShow"/> representing all retrieved popular shows.</returns>
        public async Task<List<EpisodateTvShow>> GetAllPopularShowsAsync(int maxPages = 5)
        {
            var allShows = new List<EpisodateTvShow>();

            for (int page = 1; page <= maxPages; page++)
            {
                try
                {
                    var response = await GetPopularShowsAsync(page);
                    if (response?.TvShows != null && response.TvShows.Any())
                    {
                        allShows.AddRange(response.TvShows);
                        if (page >= response.Pages) break; // Stop if last page reached
                    }
                    else
                    {
                        _logger.LogWarning("No shows found on page {Page}", page);
                        break;
                    }

                    await Task.Delay(1000); // Delay to avoid overloading the API
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching page {Page}", page);
                    break;
                }
            }

            _logger.LogInformation("Total {Count} shows retrieved from {Pages} pages", allShows.Count, maxPages);
            return allShows;
        }
    }
}
