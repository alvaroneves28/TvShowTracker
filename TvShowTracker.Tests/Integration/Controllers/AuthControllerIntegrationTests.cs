using System.Net;
using System.Text;
using System.Text.Json;
using TvShowTracker.API;
using TvShowTracker.Application.DTOs;

namespace TvShowTracker.Tests.Integration.Controllers
{
    public class AuthControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.Token);
            Assert.Equal("newuser", authResponse.User.Username);
            Assert.Equal("newuser@example.com", authResponse.User.Email);
        }

        [Fact]
        public async Task Register_WithInvalidPassword_ShouldReturnBadRequest()
        {
            // ArrangeF
            var registerDto = new RegisterDto
            {
                Username = "testuser2",
                Email = "test2@example.com",
                Password = "weak", // Password muito fraca
                ConfirmPassword = "weak"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange - Primeiro registrar um usuário
            await RegisterTestUserAsync();

            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = "Password123!"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.Token);
            Assert.Equal("loginuser", authResponse.User.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "nonexistent",
                Password = "wrongpassword"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task RegisterTestUserAsync()
        {
            var registerDto = new RegisterDto
            {
                Username = "loginuser",
                Email = "loginuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _client.PostAsync("/api/auth/register", content);
        }
    }
}