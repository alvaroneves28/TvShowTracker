using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user registration requests.
    /// Contains all necessary information to create a new user account with comprehensive validation rules.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Unique username for the new account.
        /// Serves as the primary identifier for login and user references throughout the system.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address for account verification, communication, and account recovery.
        /// Must be a valid, accessible email address that the user owns.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Secure password for account protection.
        /// Must meet complexity requirements to ensure account security against common attacks.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least: 1 lowercase letter, 1 uppercase letter, 1 number, and 1 special character")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation field to prevent user input errors during registration.
        /// Must exactly match the Password field to ensure user intention and prevent typos.
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Password and confirmation do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}