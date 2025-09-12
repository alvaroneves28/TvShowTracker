using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;

namespace TvShowTracker.API.Controllers
{
    /// <summary>
    /// Controller responsible for user authentication and registration operations.
    /// Handles user account creation, login, profile management, and availability checks.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the AuthController.
        /// </summary>
        /// <param name="userService">Service for user-related operations</param>
        /// <param name="logger">Logger instance for tracking operations and errors</param>
        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        /// <param name="registerDto">Registration data containing username, email, password, and confirmation</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        /// <response code="201">User successfully registered and authenticated</response>
        /// <response code="400">Invalid input data, password mismatch, or user already exists</response>
        /// <response code="500">Internal server error during registration process</response>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Validate model state - ensures all required fields are present and properly formatted
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Additional business logic validations beyond basic model validation

                // Ensure password confirmation matches the original password
                if (registerDto.Password != registerDto.ConfirmPassword)
                    return BadRequest("Passwords do not match");

                // Check if username is already taken to prevent duplicates
                if (await _userService.UsernameExistsAsync(registerDto.Username))
                    return BadRequest("Username already exists");

                // Check if email is already registered to prevent duplicates
                if (await _userService.EmailExistsAsync(registerDto.Email))
                    return BadRequest("Email is already in use");

                // Process registration and generate authentication response
                var response = await _userService.RegisterAsync(registerDto);

                // Return 201 Created with location header pointing to profile endpoint
                return CreatedAtAction(nameof(GetProfile), null, response);
            }
            catch (ArgumentException ex)
            {
                // Handle specific validation errors from the service layer
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log unexpected errors for debugging and monitoring
                _logger.LogError(ex, "Error registering user {Username}", registerDto.Username);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token for subsequent API calls.
        /// </summary>
        /// <param name="loginDto">Login credentials containing username and password</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        /// <response code="200">User successfully authenticated</response>
        /// <response code="400">Invalid input data format</response>
        /// <response code="401">Invalid username or password</response>
        /// <response code="500">Internal server error during login process</response>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Validate basic model state for required fields
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Attempt authentication through the user service
                var response = await _userService.LoginAsync(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                // Handle authentication failures without revealing specific reason for security
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                // Log unexpected errors while protecting sensitive information
                _logger.LogError(ex, "Error during login attempt for user {Username}", loginDto.Username);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves the profile information of the currently authenticated user.
        /// </summary>
        /// <returns>User profile information including ID, username, email, and creation date</returns>
        /// <response code="200">Profile successfully retrieved</response>
        /// <response code="401">Invalid or missing JWT token</response>
        /// <response code="404">User not found in the system</response>
        /// <response code="500">Internal server error during profile retrieval</response>
        [HttpGet("profile")]
        [Authorize] // Requires valid JWT token
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                // Extract user ID from JWT token claims for security
                var userIdClaim = User.FindFirst("userId")?.Value;

                // Validate that the user ID claim exists and is a valid integer
                if (!int.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                // Retrieve user information from the database
                var user = await _userService.GetUserByIdAsync(userId);

                // Handle case where user exists in token but not in database (rare edge case)
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log errors without exposing user-specific information in logs
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Checks if a username is available for registration.
        /// </summary>
        /// <param name="username">The username to check for availability</param>
        /// <returns>Object indicating whether the username is available</returns>
        /// <response code="200">Availability check completed successfully</response>
        /// <response code="500">Internal server error during availability check</response>
        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<object>> CheckUsername(string username)
        {
            try
            {
                // Query the database to check if username is already taken
                var exists = await _userService.UsernameExistsAsync(username);

                // Return inverted result (available = !exists)
                return Ok(new { available = !exists });
            }
            catch (Exception ex)
            {
                // Log error with username for debugging purposes
                _logger.LogError(ex, "Error checking username availability for {Username}", username);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Checks if an email address is available for registration.
        /// </summary>
        /// <param name="email">The email address to check for availability</param>
        /// <returns>Object indicating whether the email is available</returns>
        /// <response code="200">Availability check completed successfully</response>
        /// <response code="500">Internal server error during availability check</response>
        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<object>> CheckEmail(string email)
        {
            try
            {
                // Query the database to check if email is already registered
                var exists = await _userService.EmailExistsAsync(email);

                // Return inverted result (available = !exists)
                return Ok(new { available = !exists });
            }
            catch (Exception ex)
            {
                // Log error with email for debugging purposes (consider privacy implications)
                _logger.LogError(ex, "Error checking email availability for {Email}", email);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}