using TvShowTracker.Application.DTOs;

namespace TvShowTracker.Application.Interfaces
{
    /// <summary>
    /// Service interface for user account management and authentication operations.
    /// Provides comprehensive user lifecycle management including registration, authentication, and profile operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user account with comprehensive validation and security processing.
        /// Creates a complete user profile and returns authentication context for immediate login.
        /// </summary>
        /// <param name="registerDto">Complete registration data including credentials and profile information</param>
        /// <returns>Authentication response with JWT token and user profile data</returns>
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Authenticates user credentials and establishes an authenticated session.
        /// Validates credentials and returns authentication context with JWT token.
        /// </summary>
        /// <param name="loginDto">User credentials for authentication verification</param>
        /// <returns>Authentication response with JWT token and user profile data</returns>
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Retrieves user profile information by unique identifier.
        /// Provides complete user data for profile management and display purposes.
        /// </summary>
        /// <param name="id">Unique identifier of the user to retrieve</param>
        /// <returns>User profile data including public information and statistics, or null if not found</returns>
        Task<UserDto?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves user profile information by username identifier.
        /// Enables username-based user lookup for various application scenarios.
        /// </summary>
        /// <param name="username">Username of the user to retrieve</param>
        /// <returns>User profile data including public information and statistics, or null if not found</returns>
        Task<UserDto?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Checks whether a username is already taken by an existing user account.
        /// Provides real-time username availability validation for registration workflows.
        /// </summary>
        /// <param name="username">Username to check for availability</param>
        /// <returns>True if username exists, false if available for registration</returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Checks whether an email address is already registered with an existing user account.
        /// Provides email uniqueness validation for registration and account management workflows.
        /// </summary>
        /// <param name="email">Email address to check for existing registration</param>
        /// <returns>True if email exists, false if available for registration</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}