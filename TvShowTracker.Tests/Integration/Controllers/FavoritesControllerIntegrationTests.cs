using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TvShowTracker.API;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Tests.Integration.Controllers
{
    /// <summary>
    /// Integration tests for <c>FavoritesController</c>, verifying favorite management functionality.
    /// </summary>
    public class FavoritesControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoritesControllerIntegrationTests"/> class.
        /// </summary>
        public FavoritesControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Tests adding a favorite TV show with proper authentication.
        /// Expects HTTP 200 OK.
        /// </summary>
        [Fact]
        public async Task AddFavorite_WithAuthentication_ShouldReturnOk()
        {
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync("/api/favorites/1", null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests adding a favorite TV show without authentication.
        /// Expects HTTP 401 Unauthorized.
        /// </summary>
        [Fact]
        public async Task AddFavorite_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            await SeedTestDataAsync();

            var response = await _client.PostAsync("/api/favorites/1", null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        /// <summary>
        /// Tests retrieving user favorites with authentication.
        /// Expects HTTP 200 OK and a non-null favorites list.
        /// </summary>
        [Fact]
        public async Task GetUserFavorites_WithAuthentication_ShouldReturnFavorites()
        {
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Adicionar um favorito primeiro
            await _client.PostAsync("/api/favorites/1", null);

            var response = await _client.GetAsync("/api/favorites");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var favorites = JsonSerializer.Deserialize<List<TvShowDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(favorites);
        }

        /// <summary>
        /// Tests removing a favorite TV show with authentication.
        /// Expects HTTP 200 OK.
        /// </summary>
        [Fact]
        public async Task RemoveFavorite_WithAuthentication_ShouldReturnOk()
        {
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Adicionar favorito primeiro
            await _client.PostAsync("/api/favorites/1", null);

            var response = await _client.DeleteAsync("/api/favorites/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests toggling a favorite TV show (add then remove).
        /// Expects HTTP 200 OK on both operations.
        /// </summary>
        [Fact]
        public async Task ToggleFavorite_ShouldToggleCorrectly()
        {
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Primeiro toggle (adicionar)
            var response1 = await _client.PostAsync("/api/favorites/toggle/1", null);

            // Segundo toggle (remover)
            var response2 = await _client.PostAsync("/api/favorites/toggle/1", null);

            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        /// <summary>
        /// Seeds test TV show data into the in-memory database.
        /// </summary>
        private async Task SeedTestDataAsync()
        {
            using var context = _factory.GetDbContext();

            // Limpar dados existentes
            context.TvShows.RemoveRange(context.TvShows);
            context.SaveChanges();

            var tvShow = new TvShow
            {
                Id = 1,
                Name = "Test Show",
                Description = "Test description",
                StartDate = DateTime.Now.AddYears(-2),
                Status = "Ended",
                Network = "Netflix",
                ImageUrl = "https://example.com/image.jpg",
                Rating = 8.5,
                Genres = new List<string> { "Drama" },
                ShowType = "Series",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.TvShows.Add(tvShow);
            context.SaveChanges();
        }

        /// <summary>
        /// Registers a new test user and returns a JWT token for authentication.
        /// </summary>
        private async Task<string> GetAuthTokenAsync()
        {
            var registerDto = new RegisterDto
            {
                Username = $"testuser{Random.Shared.Next(1000, 9999)}",
                Email = $"test{Random.Shared.Next(1000, 9999)}@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var json = JsonSerializer.Serialize(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Auth failed: {response.StatusCode} - {responseContent}");
            }

            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return authResponse.Token;
        }
    }
}
