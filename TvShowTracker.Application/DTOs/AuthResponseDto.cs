using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object representing the response from successful authentication operations.
    /// Contains JWT token, user information, and token expiration details for authenticated sessions.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// JWT (JSON Web Token) for authenticating subsequent API requests.
        /// This token must be included in the Authorization header for protected endpoints.
        /// </summary>
        [Required(ErrorMessage = "Authentication token is required")]
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// User information for the authenticated user.
        /// Contains basic user profile data for client-side display and personalization.
        /// </summary>
        [Required(ErrorMessage = "User information is required")]
        public UserDto User { get; set; } = new();

        /// <summary>
        /// UTC timestamp indicating when the JWT token expires.
        /// Clients should monitor this value to handle token expiration gracefully.
        /// </summary>
        [Required(ErrorMessage = "Token expiration time is required")]
        public DateTime ExpiresAt { get; set; }
    }
}