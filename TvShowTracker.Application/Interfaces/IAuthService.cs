using System.Security.Claims;

namespace TvShowTracker.Application.Interfaces
{
    /// <summary>
    /// Service interface for authentication operations including JWT token management and password security.
    /// Provides centralized authentication functionality for user verification and security operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates a JSON Web Token (JWT) containing user identity claims for authentication.
        /// Creates a cryptographically signed token with user information and expiration.
        /// </summary>
        /// <param name="userId">Unique identifier for the user account</param>
        /// <param name="username">The user's username for display and identification</param>
        /// <param name="email">The user's email address for additional identification</param>
        /// <returns>A JWT token string formatted for Authorization header usage</returns>
        string GenerateJwtToken(int userId, string username, string email);

        /// <summary>
        /// Validates a JWT token and extracts user claims for authentication context.
        /// Verifies token signature, expiration, and format to ensure authenticity.
        /// </summary>
        /// <param name="token">The JWT token string to validate (without "Bearer " prefix)</param>
        /// <returns>ClaimsPrincipal containing user claims if valid, null if invalid</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Securely hashes a plain text password using cryptographically strong algorithms.
        /// Generates a salted hash suitable for secure storage and future verification.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>A hashed password string including salt and algorithm information</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a previously generated hash.
        /// Uses constant-time comparison to prevent timing attack vulnerabilities.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hashedPassword">The stored password hash for comparison</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}