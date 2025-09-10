using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registrar novo usuário
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validações adicionais
                if (registerDto.Password != registerDto.ConfirmPassword)
                    return BadRequest("As passwords não coincidem");

                if (await _userService.UsernameExistsAsync(registerDto.Username))
                    return BadRequest("Username já existe");

                if (await _userService.EmailExistsAsync(registerDto.Email))
                    return BadRequest("Email já está em uso");

                var response = await _userService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(GetProfile), null, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário {Username}", registerDto.Username);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Fazer login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Credenciais inválidas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login do usuário {Username}", loginDto.Username);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter perfil do usuário autenticado
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound("Usuário não encontrado");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil do usuário");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verificar se username está disponível
        /// </summary>
        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<object>> CheckUsername(string username)
        {
            try
            {
                var exists = await _userService.UsernameExistsAsync(username);
                return Ok(new { available = !exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar username {Username}", username);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Verificar se email está disponível
        /// </summary>
        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<object>> CheckEmail(string email)
        {
            try
            {
                var exists = await _userService.EmailExistsAsync(email);
                return Ok(new { available = !exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar email {Email}", email);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}