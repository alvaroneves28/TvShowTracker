using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TvShowTracker.API;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Tests.Integration.Controllers
{
    public class FavoritesControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public FavoritesControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task AddFavorite_WithAuthentication_ShouldReturnOk()
        {
            // Arrange
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsync("/api/favorites/1", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddFavorite_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var response = await _client.PostAsync("/api/favorites/1", null);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserFavorites_WithAuthentication_ShouldReturnFavorites()
        {
            // Arrange
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Adicionar um favorito primeiro
            await _client.PostAsync("/api/favorites/1", null);

            // Act
            var response = await _client.GetAsync("/api/favorites");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var favorites = JsonSerializer.Deserialize<List<TvShowDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(favorites);
        }

        [Fact]
        public async Task RemoveFavorite_WithAuthentication_ShouldReturnOk()
        {
            // Arrange
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Adicionar favorito primeiro
            await _client.PostAsync("/api/favorites/1", null);

            // Act
            var response = await _client.DeleteAsync("/api/favorites/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ToggleFavorite_ShouldToggleCorrectly()
        {
            // Arrange
            await SeedTestDataAsync();
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act - Primeiro toggle (adicionar)
            var response1 = await _client.PostAsync("/api/favorites/toggle/1", null);

            // Act - Segundo toggle (remover)
            var response2 = await _client.PostAsync("/api/favorites/toggle/1", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        }

        private async Task SeedTestDataAsync()
        {
            using var context = _factory.GetDbContext(); // Mudança aqui

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

        private async Task<string> GetAuthTokenAsync()
        {
            var registerDto = new RegisterDto
            {
                Username = $"testuser{Random.Shared.Next(1000, 9999)}", // Usar números em vez de GUID
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