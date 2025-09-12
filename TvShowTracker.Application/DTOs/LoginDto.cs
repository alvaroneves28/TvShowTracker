using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user authentication requests.
    /// Contains the credentials necessary for validating user identity and establishing authenticated sessions.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The unique username identifying the user account.
        /// Must match an existing user account in the system for successful authentication.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The user's secret password for authentication verification.
        /// Must match the password hash stored during account registration.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}