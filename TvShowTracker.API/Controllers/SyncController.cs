using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvShowTracker.Infrastructure.Data;
using TvShowTracker.Infrastructure.ExternalServices;

namespace TvShowTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Apenas usuários autenticados podem acessar
    [Produces("application/json")]
    public class SyncController : ControllerBase
    {
        private readonly TvShowContext _context;
        private readonly IExternalTvShowService _externalService;
        private readonly ILogger<SyncController> _logger;

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
        /// Obter estatísticas da base de dados
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetSyncStats()
        {
            try
            {
                var stats = new
                {
                    TotalShows = await _context.TvShows.CountAsync(),
                    TotalEpisodes = await _context.Episodes.CountAsync(),
                    TotalActors = await _context.Actors.CountAsync(),
                    TotalUsers = await _context.Users.CountAsync(),
                    TotalFavorites = await _context.UserFavorites.CountAsync(),
                    LastSyncTime = await _context.TvShows
                        .OrderByDescending(ts => ts.UpdatedAt)
                        .Select(ts => ts.UpdatedAt)
                        .FirstOrDefaultAsync(),
                    RecentlyAdded = await _context.TvShows
                        .Where(ts => ts.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                        .CountAsync()
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Forçar sincronização manual (apenas para desenvolvimento)
        /// </summary>
        [HttpPost("force-sync")]
        public async Task<IActionResult> ForceSync()
        {
            try
            {
                _logger.LogInformation("Sincronização manual iniciada");

                // Buscar algumas séries da API externa
                var externalShows = await _externalService.GetAllPopularShowsAsync(maxPages: 1);

                if (!externalShows.Any())
                {
                    return BadRequest("Nenhuma série obtida da API externa");
                }

                int processed = 0;
                foreach (var externalShow in externalShows.Take(5)) // Limitar a 5 para teste manual
                {
                    var existingShow = await _context.TvShows
                        .FirstOrDefaultAsync(ts => ts.Name.ToLower() == externalShow.Name.ToLower());

                    if (existingShow == null)
                    {
                        var newShow = new Core.Entities.TvShow
                        {
                            Name = externalShow.Name,
                            Description = "Sincronizado da API externa",
                            Network = externalShow.Network,
                            Status = externalShow.Status,
                            ImageUrl = externalShow.ImageThumbnailPath,
                            ShowType = "Series",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            Genres = new List<string> { "General" }
                        };

                        if (DateTime.TryParse(externalShow.StartDate, out var startDate))
                        {
                            newShow.StartDate = startDate;
                        }
                        else
                        {
                            newShow.StartDate = DateTime.UtcNow;
                        }

                        _context.TvShows.Add(newShow);
                        processed++;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Sincronização manual concluída",
                    processedShows = processed
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante sincronização manual");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Testar conexão com API externa
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestExternalConnection()
        {
            try
            {
                var response = await _externalService.GetPopularShowsAsync(1);

                if (response == null)
                {
                    return BadRequest("Não foi possível conectar à API externa");
                }

                return Ok(new
                {
                    message = "Conexão com API externa bem-sucedida",
                    totalShows = response.Total,
                    currentPage = response.Page,
                    totalPages = response.Pages,
                    showsInPage = response.TvShows?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com API externa");
                return StatusCode(500, ex.Message);
            }
        }
    }
}