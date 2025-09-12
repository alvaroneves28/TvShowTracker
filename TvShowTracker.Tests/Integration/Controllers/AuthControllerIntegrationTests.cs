using System.Net;
using System.Text;
using System.Text.Json;
using TvShowTracker.API;
using TvShowTracker.Application.DTOs;

namespace TvShowTracker.Tests.Integration.Controllers
{
    /// <summary>
    /// Integration tests for <c>AuthController</c>, verifying registration and login flows.
    /// </summary>
    public class AuthControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerIntegrationTests"/> class.
        /// </summary>
        public AuthControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Ensures that registering a new user with valid data returns HTTP 201 Created
        /// and provides a valid authentication token.
        /// </summary>
        [Fact]
        public async Task Register_WithValidData_ShouldReturnCreated()
        {
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.Token);
            Assert.Equal("newuser", authResponse.User.Username);
            Assert.Equal("newuser@example.com", authResponse.User.Email);
        }

        /// <summary>
        /// Ensures that registering a user with an invalid or weak password returns HTTP 400 BadRequest.
        /// </summary>
        [Fact]
        public async Task Register_WithInvalidPassword_ShouldReturnBadRequest()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser2",
                Email = "test2@example.com",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Ensures that logging in with valid credentials returns HTTP 200 OK
        /// and provides a valid authentication token.
        /// </summary>
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            await RegisterTestUserAsync();

            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = "Password123!"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(authResponse);
            Assert.NotNull(authResponse.Token);
            Assert.Equal("loginuser", authResponse.User.Username);
        }

        /// <summary>
        /// Ensures that logging in with invalid credentials returns HTTP 401 Unauthorized.
        /// </summary>
        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginDto = new LoginDto
            {
                Username = "nonexistent",
                Password = "wrongpassword"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        /// <summary>
        /// Registers a test user for use in login tests.
        /// </summary>
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
