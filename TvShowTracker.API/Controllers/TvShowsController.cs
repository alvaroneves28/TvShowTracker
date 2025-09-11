using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TvShowsController : ControllerBase
    {
        private readonly ITvShowService _tvShowService;
        private readonly ILogger<TvShowsController> _logger;

        public TvShowsController(ITvShowService tvShowService, ILogger<TvShowsController> logger)
        {
            _tvShowService = tvShowService;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as séries com paginação e filtros
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TvShowDto>>> GetTvShows([FromQuery] QueryParameters parameters)
        {
            try
            {
                var result = await _tvShowService.GetAllTvShowsAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter séries");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter série por ID com detalhes completos
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TvShowDetailDto>> GetTvShow(int id)
        {
            try
            {
                // Obter userId se autenticado
                int? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst("userId")?.Value;
                    if (int.TryParse(userIdClaim, out var parsedUserId))
                        userId = parsedUserId;
                }

                var tvShow = await _tvShowService.GetTvShowByIdAsync(id, userId);
                if (tvShow == null)
                    return NotFound($"Série com ID {id} não encontrada");

                return Ok(tvShow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter série {TvShowId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Criar nova série
        /// </summary>
        [HttpPost]
        [Authorize] // Apenas usuários autenticados podem criar séries
        public async Task<ActionResult<TvShowDto>> CreateTvShow([FromBody] CreateTvShowDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tvShow = await _tvShowService.CreateTvShowAsync(createDto);
                return CreatedAtAction(nameof(GetTvShow), new { id = tvShow.Id }, tvShow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar série");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Atualizar série existente
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<TvShowDto>> UpdateTvShow(int id, [FromBody] CreateTvShowDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var tvShow = await _tvShowService.UpdateTvShowAsync(id, updateDto);
                if (tvShow == null)
                    return NotFound($"Série com ID {id} não encontrada");

                return Ok(tvShow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar série {TvShowId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Deletar série
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteTvShow(int id)
        {
            try
            {
                var success = await _tvShowService.DeleteTvShowAsync(id);
                if (!success)
                    return NotFound($"Série com ID {id} não encontrada");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar série {TvShowId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Buscar séries por texto
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> SearchTvShows([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    return BadRequest("Query de busca não pode estar vazia");

                var tvShows = await _tvShowService.SearchTvShowsAsync(query);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar séries com query: {Query}", query);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter séries por gênero
        /// </summary>
        [HttpGet("genre/{genre}")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetTvShowsByGenre(string genre)
        {
            try
            {
                var tvShows = await _tvShowService.GetTvShowsByGenreAsync(genre);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter séries do gênero: {Genre}", genre);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter séries por tipo
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<TvShowDto>>> GetTvShowsByType(string type)
        {
            try
            {
                var tvShows = await _tvShowService.GetTvShowsByTypeAsync(type);
                return Ok(tvShows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter séries do tipo: {Type}", type);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter episódios de uma série
        /// </summary>
        [HttpGet("{id:int}/episodes")]
        public async Task<ActionResult<IEnumerable<EpisodeDto>>> GetTvShowEpisodes(int id)
        {
            try
            {
                var episodes = await _tvShowService.GetTvShowEpisodesAsync(id);
                return Ok(episodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter episódios da série {TvShowId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter atores de uma série
        /// </summary>
        [HttpGet("{id:int}/actors")]
        public async Task<ActionResult<IEnumerable<ActorDto>>> GetTvShowActors(int id)
        {
            try
            {
                var actors = await _tvShowService.GetTvShowActorsAsync(id);
                return Ok(actors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter atores da série {TvShowId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
