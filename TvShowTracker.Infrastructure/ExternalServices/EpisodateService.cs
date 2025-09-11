using Microsoft.Extensions.Logging;
using System.Text.Json;
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
                _logger.LogInformation("=== TESTE EPISODATE DEBUG ===");
                _logger.LogInformation("URL: {Url}", url);
                _logger.LogInformation("User-Agent: {UserAgent}", _httpClient.DefaultRequestHeaders.UserAgent.ToString());
                _logger.LogInformation("Timeout: {Timeout}", _httpClient.Timeout);

                // Tentar com headers adicionais
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                request.Headers.Add("Cache-Control", "no-cache");

                _logger.LogInformation("Enviando request...");

                var response = await _httpClient.SendAsync(request);

                _logger.LogInformation("Status Code: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Content Type: {ContentType}", response.Content.Headers.ContentType);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro HTTP {StatusCode}: {Content}", response.StatusCode, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response Length: {Length} chars", jsonContent.Length);
                _logger.LogInformation("Response Preview: {Preview}",
                    jsonContent.Length > 200 ? jsonContent.Substring(0, 200) + "..." : jsonContent);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    _logger.LogError("Response content é vazio");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<EpisodateResponse>(jsonContent, options);

                _logger.LogInformation("Deserialization concluída. Total shows: {Count}",
                    result?.TvShows?.Count ?? 0);

                if (result?.TvShows?.Any() == true)
                {
                    _logger.LogInformation("Primeira série: {ShowName} - {Network}",
                        result.TvShows.First().Name, result.TvShows.First().Network);
                }

                return result;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Erro de HTTP request para {Page}: {Message}", page, httpEx.Message);
                return null;
            }
            catch (TaskCanceledException timeoutEx)
            {
                _logger.LogError(timeoutEx, "Timeout ao buscar séries da página {Page}", page);
                return null;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Erro ao deserializar JSON da página {Page}: {Message}", page, jsonEx.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro geral ao buscar séries da página {Page}: {Message}", page, ex.Message);
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
