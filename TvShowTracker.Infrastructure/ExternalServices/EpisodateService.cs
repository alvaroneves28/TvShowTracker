using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TvShowTracker.Infrastructure.ExternalServices.Models;

namespace TvShowTracker.Infrastructure.ExternalServices
{
    public class EpisodateService : IExternalTvShowService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EpisodateService> _logger;
        private const string BaseUrl = "https://www.episodate.com/api";

        public EpisodateService(HttpClient httpClient, ILogger<EpisodateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            // Configurar headers
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TvShowTracker/1.0");
        }

        public async Task<EpisodateResponse?> GetPopularShowsAsync(int page = 1)
        {
            try
            {
                var url = $"{BaseUrl}/most-popular?page={page}";
                _logger.LogInformation("Buscando séries populares da página {Page}", page);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erro ao buscar séries: {StatusCode}", response.StatusCode);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<EpisodateResponse>(jsonContent, options);
                _logger.LogInformation("Encontradas {Count} séries na página {Page}",
                    result?.TvShows?.Count ?? 0, page);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar séries populares da página {Page}", page);
                return null;
            }
        }

        public async Task<EpisodateTvShowDetail?> GetShowDetailsAsync(int showId)
        {
            try
            {
                var url = $"{BaseUrl}/show-details?q={showId}";
                _logger.LogInformation("Buscando detalhes da série {ShowId}", showId);

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erro ao buscar detalhes da série {ShowId}: {StatusCode}",
                        showId, response.StatusCode);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<EpisodateTvShowDetail>(jsonContent, options);
                _logger.LogInformation("Detalhes da série {ShowId} obtidos com sucesso", showId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes da série {ShowId}", showId);
                return null;
            }
        }

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

                        // Se chegamos à última página, parar
                        if (page >= response.Pages)
                            break;
                    }
                    else
                    {
                        _logger.LogWarning("Nenhuma série encontrada na página {Page}", page);
                        break;
                    }

                    // Delay para não sobrecarregar a API
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar página {Page}", page);
                    break;
                }
            }

            _logger.LogInformation("Total de {Count} séries obtidas em {Pages} páginas",
                allShows.Count, maxPages);

            return allShows;
        }
    }
}
