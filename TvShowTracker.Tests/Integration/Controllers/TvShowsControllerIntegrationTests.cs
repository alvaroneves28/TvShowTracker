using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TvShowTracker.API;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Core.Entities;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Tests.Integration.Controllers
{
    public class TvShowsControllerIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TvShowsControllerIntegrationTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetTvShows_ShouldReturnOkWithPaginatedData()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var response = await _client.GetAsync("/api/tvshows?page=1&pageSize=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PagedResultDto<TvShowDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(result);
            Assert.True(result.Data.Any());
        }

        [Fact]
        public async Task GetTvShow_WithValidId_ShouldReturnTvShow()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var response = await _client.GetAsync("/api/tvshows/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var tvShow = JsonSerializer.Deserialize<TvShowDetailDto>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(tvShow);
            Assert.Equal("Test Show 1", tvShow.Name);
        }

        [Fact]
        public async Task GetTvShow_WithInvalidId_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/tvshows/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateTvShow_WithAuthentication_ShouldCreateTvShow()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createDto = new CreateTvShowDto
            {
                Name = "New Test Show",
                Description = "Test description",
                StartDate = DateTime.Now,
                Status = "Running",
                Network = "Netflix",
                Rating = 8.5,
                Genres = new List<string> { "Drama" },
                ShowType = "Series"
            };

            var json = JsonSerializer.Serialize(createDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/tvshows", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var createdShow = JsonSerializer.Deserialize<TvShowDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(createdShow);
            Assert.Equal("New Test Show", createdShow.Name);
        }

        [Fact]
        public async Task CreateTvShow_WithoutAuthentication_ShouldReturnUnauthorized()
        {
            // Arrange
            var createDto = new CreateTvShowDto
            {
                Name = "New Test Show",
                Description = "Test description",
                StartDate = DateTime.Now,
                Status = "Running",
                Network = "Netflix",
                Rating = 8.5,
                Genres = new List<string> { "Drama" },
                ShowType = "Series"
            };

            var json = JsonSerializer.Serialize(createDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/tvshows", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SearchTvShows_ShouldReturnMatchingShows()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var response = await _client.GetAsync("/api/tvshows/search?query=Test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var shows = JsonSerializer.Deserialize<List<TvShowDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(shows);
            Assert.True(shows.Any());
            Assert.All(shows, show => Assert.Contains("Test", show.Name));
        }

        private async Task SeedTestDataAsync()
        {
            using var context = _factory.GetDbContext(); // Mudança aqui - remover await

            // Limpar dados existentes
            context.TvShows.RemoveRange(context.TvShows);
            context.SaveChanges();

            var tvShows = new List<TvShow>
    {
        new TvShow
        {
            Id = 1,
            Name = "Test Show 1",
            Description = "Test description 1",
            StartDate = DateTime.Now.AddYears(-2),
            Status = "Ended",
            Network = "Netflix",
            ImageUrl = "https://example.com/image1.jpg",
            Rating = 8.5,
            Genres = new List<string> { "Drama" },
            ShowType = "Series",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new TvShow
        {
            Id = 2,
            Name = "Test Show 2",
            Description = "Test description 2",
            StartDate = DateTime.Now.AddYears(-1),
            Status = "Running",
            Network = "HBO",
            ImageUrl = "https://example.com/image2.jpg",
            Rating = 9.0,
            Genres = new List<string> { "Comedy" },
            ShowType = "Series",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };

            context.TvShows.AddRange(tvShows);
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