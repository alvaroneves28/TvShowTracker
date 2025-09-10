using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todos os endpoints requerem autenticação
    [Produces("application/json")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(IFavoriteService favoriteService, ILogger<FavoritesController> logger)
        {
            _favoriteService = favoriteService;
            _logger = logger;
        }

        /// <summary>
        /// Obter favoritos do usuário autenticado
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetUserFavorites()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var favorites = await _favoriteService.GetUserFavoritesAsync(userId.Value);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter favoritos do usuário");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Adicionar série aos favoritos
        /// </summary>
        [HttpPost("{tvShowId:int}")]
        public async Task<IActionResult> AddFavorite(int tvShowId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _favoriteService.AddFavoriteAsync(userId.Value, tvShowId);
                if (!success)
                    return BadRequest("Não foi possível adicionar aos favoritos. Série não encontrada ou já é favorita.");

                return Ok(new { message = "Série adicionada aos favoritos com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar série {TvShowId} aos favoritos", tvShowId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Remover série dos favoritos
        /// </summary>
        [HttpDelete("{tvShowId:int}")]
        public async Task<IActionResult> RemoveFavorite(int tvShowId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var success = await _favoriteService.RemoveFavoriteAsync(userId.Value, tvShowId);
                if (!success)
                    return BadRequest("Não foi possível remover dos favoritos. Série não está nos favoritos.");

                return Ok(new { message = "Série removida dos favoritos com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover série {TvShowId} dos favoritos", tvShowId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verificar se série é favorita
        /// </summary>
        [HttpGet("check/{tvShowId:int}")]
        public async Task<ActionResult<object>> IsFavorite(int tvShowId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tvShowId);
                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se série {TvShowId} é favorita", tvShowId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter contagem de favoritos do usuário
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<object>> GetFavoritesCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var count = await _favoriteService.GetFavoritesCountAsync(userId.Value);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter contagem de favoritos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Alternar estado de favorito (toggle)
        /// </summary>
        [HttpPost("toggle/{tvShowId:int}")]
        public async Task<ActionResult<object>> ToggleFavorite(int tvShowId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var isFavorite = await _favoriteService.IsFavoriteAsync(userId.Value, tvShowId);

                bool success;
                string message;

                if (isFavorite)
                {
                    success = await _favoriteService.RemoveFavoriteAsync(userId.Value, tvShowId);
                    message = "Série removida dos favoritos";
                }
                else
                {
                    success = await _favoriteService.AddFavoriteAsync(userId.Value, tvShowId);
                    message = "Série adicionada aos favoritos";
                }

                if (!success)
                    return BadRequest("Não foi possível alterar o estado de favorito");

                return Ok(new
                {
                    message,
                    isFavorite = !isFavorite
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alternar favorito da série {TvShowId}", tvShowId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Método auxiliar para obter ID do usuário autenticado
        /// </summary>
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}