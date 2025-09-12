using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TvShowTracker.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace TvShowTracker.Application.Services
{
    /// <summary>
    /// Service implementation for authentication operations including JWT token management and password security.
    /// Provides secure authentication functionality with industry-standard security practices.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        /// <summary>
        /// Initializes the AuthService with JWT configuration settings.
        /// Validates all required JWT configuration parameters during construction.
        /// </summary>
        /// <param name="configuration">Application configuration containing JWT settings</param>
        /// <exception cref="ArgumentNullException">Thrown when required JWT configuration is missing</exception>
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Validate and extract required JWT configuration with descriptive error messages
            _jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT signing key not configured");
            _jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "JWT issuer not configured");
            _jwtAudience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "JWT audience not configured");
        }

        /// <summary>
        /// Generates a secure JWT token containing user identity claims with 24-hour expiration.
        /// Creates a cryptographically signed token for stateless authentication.
        /// </summary>
        /// <param name="userId">Unique identifier for the user account</param>
        /// <param name="username">The user's username for display and identification</param>
        /// <param name="email">The user's email address for additional verification</param>
        /// <returns>A JWT token string ready for Authorization header usage</returns>
        public string GenerateJwtToken(int userId, string username, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            // Define user claims for token payload
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),    // Standard user identifier
                new(ClaimTypes.Name, username),                       // Username for display
                new(ClaimTypes.Email, email),                         // Email for verification
                new("userId", userId.ToString())                      // Custom claim for easy access
            };

            // Configure token with security parameters and expiration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),              // 24-hour token lifetime
                Issuer = _jwtIssuer,                                 // Token issuer for validation
                Audience = _jwtAudience,                             // Token audience for scope
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)          // HMAC SHA-256 signing
            };

            // Create and serialize the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token and extracts user claims for authentication context.
        /// Performs comprehensive security validation including signature, expiration, and issuer verification.
        /// </summary>
        /// <param name="token">The JWT token string to validate (without "Bearer " prefix)</param>
        /// <returns>ClaimsPrincipal containing user claims if valid, null if validation fails</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtKey);

                // Validate token with comprehensive security parameters
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,                 // Verify signature integrity
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,                           // Verify token issuer
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,                         // Verify token audience
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,                         // Check expiration
                    ClockSkew = TimeSpan.Zero                        // No tolerance for clock skew
                }, out SecurityToken validatedToken);

                // Extract claims from validated token
                var jwtToken = (JwtSecurityToken)validatedToken;
                return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "jwt"));
            }
            catch
            {
                // Return null for any validation failure - secure error handling
                // Does not expose specific validation failure reasons for security
                return null;
            }
        }

        /// <summary>
        /// Securely hashes a plain text password using BCrypt with salt rounds of 12.
        /// Generates a cryptographically strong hash suitable for secure storage.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>A BCrypt hash string including salt and algorithm information</returns>
        public string HashPassword(string password)
        {
            // Generate BCrypt hash with 12 salt rounds for strong security
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        /// <summary>
        /// Verifies a plain text password against a BCrypt hash using constant-time comparison.
        /// Implements timing attack protection through BCrypt's built-in security features.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hashedPassword">The stored BCrypt hash for comparison</param>
        /// <returns>True if the password matches the hash, false otherwise</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Use BCrypt's constant-time verification for security
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}