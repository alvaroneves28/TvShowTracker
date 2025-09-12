using Moq;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.Services;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using AutoMapper;

namespace TvShowTracker.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="FavoriteService"/>.
    /// </summary>
    public class FavoriteServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly FavoriteService _favoriteService;

        public FavoriteServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            SetupMapperMocks();

            _favoriteService = new FavoriteService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Sets up AutoMapper mock to map TvShow entities to TvShowDto objects.
        /// </summary>
        private void SetupMapperMocks()
        {
            _mapperMock.Setup(m => m.Map<IEnumerable<TvShowDto>>(It.IsAny<IEnumerable<TvShow>>()))
                .Returns((IEnumerable<TvShow> source) => source.Select(s => new TvShowDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    StartDate = s.StartDate,
                    Status = s.Status,
                    Network = s.Network,
                    ImageUrl = s.ImageUrl,
                    Rating = s.Rating,
                    Genres = s.Genres,
                    ShowType = s.ShowType,
                    EpisodeCount = s.Episodes?.Count ?? 0
                }));
        }

        /// <summary>
        /// Tests adding a favorite for a user when the TV show exists and is not already a favorite.
        /// </summary>
        [Fact]
        public async Task AddFavoriteAsync_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            int userId = 1;
            int tvShowId = 1;

            var tvShow = new TvShow { Id = tvShowId, Name = "Test Show" };

            _unitOfWorkMock.Setup(x => x.TvShows.GetByIdAsync(tvShowId))
                .ReturnsAsync(tvShow);
            _unitOfWorkMock.Setup(x => x.UserFavorites.IsFavoriteAsync(userId, tvShowId))
                .ReturnsAsync(false);

            // Act
            var result = await _favoriteService.AddFavoriteAsync(userId, tvShowId);

            // Assert
            Assert.True(result);
            _unitOfWorkMock.Verify(x => x.UserFavorites.AddFavoriteAsync(userId, tvShowId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Tests retrieving a user's favorite TV shows.
        /// </summary>
        [Fact]
        public async Task GetUserFavoritesAsync_ShouldReturnFavoriteShows()
        {
            // Arrange
            int userId = 1;
            var favoriteShows = new List<TvShow>
            {
                new TvShow
                {
                    Id = 1,
                    Name = "Favorite Show 1",
                    Description = "Test",
                    StartDate = DateTime.Now,
                    Status = "Running",
                    Network = "Netflix",
                    Rating = 8.0,
                    Genres = new List<string> { "Drama" },
                    ShowType = "Series"
                }
            };

            _unitOfWorkMock.Setup(x => x.UserFavorites.GetUserFavoritesAsync(userId))
                .ReturnsAsync(favoriteShows);

            // Act
            var result = await _favoriteService.GetUserFavoritesAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Favorite Show 1", result.First().Name);
        }
    }
}
