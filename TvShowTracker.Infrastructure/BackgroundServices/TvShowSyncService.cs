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
    public class TvShowSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TvShowSyncService> _logger;
        private readonly TimeSpan _syncInterval;

        public TvShowSyncService(
            IServiceProvider serviceProvider,
            ILogger<TvShowSyncService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Configurar intervalo de sincronização (padrão: 6 horas)
            var intervalHours = configuration.GetSection("BackgroundSync")["IntervalHours"];
            var hours = int.TryParse(intervalHours, out var parsedHours) ? parsedHours : 6;
            _syncInterval = TimeSpan.FromHours(hours);

            _logger.LogInformation("TvShowSyncService configurado com intervalo de {Hours} horas", hours);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TvShowSyncService iniciado");

            // Executar sincronização inicial após 30 segundos
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncTvShowsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro durante a sincronização de séries");
                }

                // Aguardar próximo ciclo
                try
                {
                    await Task.Delay(_syncInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Serviço sendo parado
                    break;
                }
            }

            _logger.LogInformation("TvShowSyncService parado");
        }

        private async Task SyncTvShowsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando sincronização de séries...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TvShowContext>();
            var externalService = scope.ServiceProvider.GetRequiredService<IExternalTvShowService>();

            try
            {
                // Obter séries populares da API externa
                var externalShows = await externalService.GetAllPopularShowsAsync(maxPages: 3);

                if (!externalShows.Any())
                {
                    _logger.LogWarning("Nenhuma série obtida da API externa");
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

                        // Delay para não sobrecarregar a API
                        if (processedShows % 5 == 0)
                        {
                            await Task.Delay(2000, cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar série {ShowName} (ID: {ShowId})",
                            externalShow.Name, externalShow.Id);
                    }
                }

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Sincronização concluída: {New} novas séries, {Updated} atualizadas, {Total} processadas",
                    newShows, updatedShows, processedShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a sincronização");
            }
        }

        private async Task<ProcessResult> ProcessTvShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            EpisodateTvShow externalShow)
        {
            var result = new ProcessResult();

            // Verificar se a série já existe (por nome, pois IDs externos podem não coincidir)
            var existingShow = await context.TvShows
                .FirstOrDefaultAsync(ts => ts.Name.ToLower() == externalShow.Name.ToLower());

            if (existingShow != null)
            {
                // Atualizar série existente se necessário
                if (ShouldUpdateShow(existingShow, externalShow))
                {
                    await UpdateExistingShowAsync(context, externalService, existingShow, externalShow);
                    result.UpdatedShows = 1;
                }
                return result;
            }

            // Criar nova série
            await CreateNewShowAsync(context, externalService, externalShow);
            result.NewShows = 1;
            return result;
        }

        private bool ShouldUpdateShow(TvShow existingShow, EpisodateTvShow externalShow)
        {
            // Atualizar se foi modificada há mais de 1 dia
            return DateTime.UtcNow - existingShow.UpdatedAt > TimeSpan.FromDays(1);
        }

        private async Task UpdateExistingShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            TvShow existingShow,
            EpisodateTvShow externalShow)
        {
            try
            {
                // Buscar detalhes atualizados
                var showDetails = await externalService.GetShowDetailsAsync(externalShow.Id);
                if (showDetails?.TvShow != null)
                {
                    var details = showDetails.TvShow;

                    // Atualizar campos que podem ter mudado
                    existingShow.Status = details.Status;
                    existingShow.Network = details.Network;
                    existingShow.UpdatedAt = DateTime.UtcNow;

                    if (double.TryParse(details.Rating, out var rating))
                    {
                        existingShow.Rating = rating;
                    }

                    _logger.LogDebug("Série atualizada: {ShowName}", existingShow.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar série {ShowName}", existingShow.Name);
            }
        }

        private async Task CreateNewShowAsync(
            TvShowContext context,
            IExternalTvShowService externalService,
            EpisodateTvShow externalShow)
        {
            try
            {
                // Buscar detalhes completos da série
                var showDetails = await externalService.GetShowDetailsAsync(externalShow.Id);

                var tvShow = new TvShow
                {
                    Name = externalShow.Name,
                    Description = showDetails?.TvShow?.Description ?? "Descrição não disponível",
                    Network = externalShow.Network,
                    Status = externalShow.Status,
                    ImageUrl = externalShow.ImageThumbnailPath,
                    ShowType = "Series", // Padrão
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Genres = showDetails?.TvShow?.Genres ?? new List<string> { "Unknown" }
                };

                // Parse da data de início
                if (DateTime.TryParse(externalShow.StartDate, out var startDate))
                {
                    tvShow.StartDate = startDate;
                }
                else
                {
                    tvShow.StartDate = DateTime.UtcNow; // Fallback
                }

                // Parse do rating
                if (double.TryParse(showDetails?.TvShow?.Rating, out var rating))
                {
                    tvShow.Rating = Math.Max(0, Math.Min(10, rating)); // Garantir que está entre 0-10
                }

                context.TvShows.Add(tvShow);

                _logger.LogDebug("Nova série criada: {ShowName}", tvShow.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar nova série {ShowName}", externalShow.Name);
            }
        }

        private class ProcessResult
        {
            public int NewShows { get; set; } = 0;
            public int UpdatedShows { get; set; } = 0;
        }
    }
}