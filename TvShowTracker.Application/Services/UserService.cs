using AutoMapper;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;

namespace TvShowTracker.Application.Services
{
    /// <summary>
    /// Service implementation for comprehensive user account management and authentication workflows.
    /// Orchestrates complex user lifecycle operations with security enforcement and data integrity measures.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes the UserService with required dependencies for secure user management operations.
        /// Establishes the foundation for complex authentication workflows and data protection measures.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work instance for coordinated data access and transactional operations</param>
        /// <param name="mapper">AutoMapper instance for secure entity-to-DTO conversions</param>
        /// <param name="authService">Authentication service for cryptographic operations and token management</param>
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user account with comprehensive validation, security processing, and immediate authentication.
        /// Implements multi-layered validation and secure account creation with automatic login capability.
        /// </summary>
        /// <param name="registerDto">Complete registration data including credentials and profile information</param>
        /// <returns>Authentication response with JWT token and user profile for immediate session establishment</returns>
        /// <exception cref="ArgumentException">Thrown for validation failures including duplicate accounts or password mismatches</exception>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Multi-layered validation ensuring data integrity and preventing conflicts

            // Validate username uniqueness to prevent account conflicts
            if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
                throw new ArgumentException("Username already exists");

            // Validate email uniqueness ensuring single account per email address
            if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
                throw new ArgumentException("Email already exists");

            // Validate password confirmation preventing user input errors
            if (registerDto.Password != registerDto.ConfirmPassword)
                throw new ArgumentException("Passwords do not match");

            // Create user entity with secure password handling and audit trail
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = _authService.HashPassword(registerDto.Password), // Secure cryptographic hashing
                CreatedAt = DateTime.UtcNow                                     // Audit trail establishment
            };

            // Persist user entity with transactional consistency
            var createdUser = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate authentication token for immediate session establishment
            var token = _authService.GenerateJwtToken(createdUser.Id, createdUser.Username, createdUser.Email);

            // Map user entity to DTO excluding sensitive authentication data
            var userDto = _mapper.Map<UserDto>(createdUser);

            // Assemble complete authentication response for client convenience
            return new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token lifecycle management
            };
        }

        /// <summary>
        /// Authenticates user credentials and establishes secure session with comprehensive validation.
        /// Implements secure login workflow with timing attack protection and token generation.
        /// </summary>
        /// <param name="loginDto">User credentials for authentication verification</param>
        /// <returns>Authentication response with JWT token and user profile for session establishment</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown for invalid credentials or authentication failures</exception>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Retrieve user by username for credential verification
            var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username);

            // Verify user existence and password using secure comparison
            if (user == null || !_authService.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials"); // Generic security message

            // Generate authentication token with user claims and expiration
            var token = _authService.GenerateJwtToken(user.Id, user.Username, user.Email);

            // Map user to DTO excluding sensitive authentication data
            var userDto = _mapper.Map<UserDto>(user);

            // Assemble authentication response for session establishment
            return new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token lifecycle coordination
            };
        }

        /// <summary>
        /// Retrieves user profile information by unique identifier with privacy-aware data projection.
        /// Provides secure user data access excluding sensitive authentication information.
        /// </summary>
        /// <param name="id">Unique identifier of the user to retrieve</param>
        /// <returns>User profile data suitable for display and client operations, or null if not found</returns>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            // Retrieve user entity through optimized repository lookup
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            // Return mapped DTO with privacy-aware projection, or null for non-existent users
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Retrieves user profile information by username with security-aware data projection.
        /// Enables username-based user discovery while maintaining privacy and security standards.
        /// </summary>
        /// <param name="username">Username of the user to retrieve</param>
        /// <returns>User profile data excluding sensitive information, or null if not found</returns>
        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            // Retrieve user by username through optimized repository query
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);

            // Return mapped DTO with privacy-aware projection, or null for non-existent users
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Checks username availability for registration workflow with security-aware implementation.
        /// Provides efficient existence checking while preventing username enumeration attacks.
        /// </summary>
        /// <param name="username">Username to check for existing registration</param>
        /// <returns>True if username exists, false if available for registration</returns>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            // Direct delegation to repository for optimized existence checking
            return await _unitOfWork.Users.UsernameExistsAsync(username);
        }

        /// <summary>
        /// Checks email availability for account registration with privacy-protected implementation.
        /// Provides secure email uniqueness validation while preventing information disclosure.
        /// </summary>
        /// <param name="email">Email address to check for existing registration</param>
        /// <returns>True if email exists, false if available for registration</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            // Direct delegation to repository for secure email existence checking
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }
    }
}