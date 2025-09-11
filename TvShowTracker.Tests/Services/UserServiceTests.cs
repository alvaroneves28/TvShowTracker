using Moq;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Interfaces;
using TvShowTracker.Application.Services;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using AutoMapper;

namespace TvShowTracker.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _authServiceMock = new Mock<IAuthService>();
            _mapperMock = new Mock<IMapper>();

            SetupMapperMocks();

            _userService = new UserService(_unitOfWorkMock.Object, _mapperMock.Object, _authServiceMock.Object);
        }

        private void SetupMapperMocks()
        {
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns((User source) => new UserDto
                {
                    Id = source.Id,
                    Username = source.Username,
                    Email = source.Email,
                    CreatedAt = source.CreatedAt,
                    FavoritesCount = source.Favorites?.Count ?? 0
                });
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var hashedPassword = "hashedPassword123";
            var token = "jwt-token";

            _unitOfWorkMock.Setup(x => x.Users.UsernameExistsAsync(registerDto.Username))
                .ReturnsAsync(false);
            _unitOfWorkMock.Setup(x => x.Users.EmailExistsAsync(registerDto.Email))
                .ReturnsAsync(false);
            _authServiceMock.Setup(x => x.HashPassword(registerDto.Password))
                .Returns(hashedPassword);
            _authServiceMock.Setup(x => x.GenerateJwtToken(It.IsAny<int>(), registerDto.Username, registerDto.Email))
                .Returns(token);

            var createdUser = new User
            {
                Id = 1,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWorkMock.Setup(x => x.Users.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _userService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(registerDto.Username, result.User.Username);
            Assert.Equal(registerDto.Email, result.User.Email);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = 1,
                Username = loginDto.Username,
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                CreatedAt = DateTime.UtcNow
            };

            var token = "jwt-token";

            _unitOfWorkMock.Setup(x => x.Users.GetByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);
            _authServiceMock.Setup(x => x.VerifyPassword(loginDto.Password, user.PasswordHash))
                .Returns(true);
            _authServiceMock.Setup(x => x.GenerateJwtToken(user.Id, user.Username, user.Email))
                .Returns(token);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token, result.Token);
            Assert.Equal(user.Username, result.User.Username);
        }
    }
}