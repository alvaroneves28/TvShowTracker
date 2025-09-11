using Moq;
using TvShowTracker.Application.DTOs;
using TvShowTracker.Application.DTOs.Common;
using TvShowTracker.Application.Services;
using TvShowTracker.Core.Entities;
using TvShowTracker.Core.Interfaces;
using AutoMapper;

namespace TvShowTracker.Tests.Services
{
    public class TvShowServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TvShowService _tvShowService;

        public TvShowServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            // Configurar os mocks do mapper
            SetupMapperMocks();

            _tvShowService = new TvShowService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        private void SetupMapperMocks()
        {
            // TvShow -> TvShowDto
            _mapperMock.Setup(m => m.Map<TvShowDto>(It.IsAny<TvShow>()))
                .Returns((TvShow source) => new TvShowDto
                {
                    Id = source.Id,
                    Name = source.Name,
                    Description = source.Description,
                    StartDate = source.StartDate,
                    Status = source.Status,
                    Network = source.Network,
                    ImageUrl = source.ImageUrl,
                    Rating = source.Rating,
                    Genres = source.Genres,
                    ShowType = source.ShowType,
                    EpisodeCount = source.Episodes?.Count ?? 0
                });

            // IEnumerable<TvShow> -> IEnumerable<TvShowDto>
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

            // TvShow -> TvShowDetailDto
            _mapperMock.Setup(m => m.Map<TvShowDetailDto>(It.IsAny<TvShow>()))
                .Returns((TvShow source) => new TvShowDetailDto
                {
                    Id = source.Id,
                    Name = source.Name,
                    Description = source.Description,
                    StartDate = source.StartDate,
                    Status = source.Status,
                    Network = source.Network,
                    ImageUrl = source.ImageUrl,
                    Rating = source.Rating,
                    Genres = source.Genres,
                    ShowType = source.ShowType,
                    EpisodeCount = source.Episodes?.Count ?? 0,
                    Episodes = source.Episodes?.Select(e => new EpisodeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Season = e.Season,
                        EpisodeNumber = e.EpisodeNumber,
                        AirDate = e.AirDate,
                        Summary = e.Summary,
                        Rating = e.Rating
                    }).ToList() ?? new List<EpisodeDto>(),
                    Actors = source.Actors?.Select(a => new ActorDto
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Character = a.Character,
                        ImageUrl = a.ImageUrl
                    }).ToList() ?? new List<ActorDto>(),
                    IsFavorite = false
                });

            // CreateTvShowDto -> TvShow
            _mapperMock.Setup(m => m.Map<TvShow>(It.IsAny<CreateTvShowDto>()))
                .Returns((CreateTvShowDto source) => new TvShow
                {
                    Name = source.Name,
                    Description = source.Description,
                    StartDate = source.StartDate,
                    Status = source.Status,
                    Network = source.Network,
                    ImageUrl = source.ImageUrl,
                    Rating = source.Rating,
                    Genres = source.Genres,
                    ShowType = source.ShowType
                });
        }

        [Fact]
        public async Task GetAllTvShowsAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var parameters = new QueryParameters { Page = 1, PageSize = 10 };
            var tvShows = new List<TvShow>
            {
                new TvShow
                {
                    Id = 1,
                    Name = "Breaking Bad",
                    Description = "Test description",
                    StartDate = DateTime.Now,
                    Status = "Ended",
                    Network = "AMC",
                    Rating = 9.5,
                    Genres = new List<string> { "Drama" },
                    ShowType = "Series",
                    Episodes = new List<Episode>()
                }
            };

            _unitOfWorkMock.Setup(x => x.TvShows.GetPagedAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((tvShows, 1));

            // Act
            var result = await _tvShowService.GetAllTvShowsAsync(parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("Breaking Bad", result.Data.First().Name);
        }

        [Fact]
        public async Task GetTvShowByIdAsync_WithValidId_ShouldReturnTvShowDetail()
        {
            // Arrange
            var tvShow = new TvShow
            {
                Id = 1,
                Name = "Breaking Bad",
                Description = "Test description",
                StartDate = DateTime.Now,
                Status = "Ended",
                Network = "AMC",
                Rating = 9.5,
                Genres = new List<string> { "Drama" },
                ShowType = "Series",
                Episodes = new List<Episode>(),
                Actors = new List<Actor>()
            };

            _unitOfWorkMock.Setup(x => x.TvShows.GetByIdWithEpisodesAsync(1))
                .ReturnsAsync(tvShow);
            _unitOfWorkMock.Setup(x => x.TvShows.GetByIdWithActorsAsync(1))
                .ReturnsAsync(tvShow);

            // Act
            var result = await _tvShowService.GetTvShowByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Breaking Bad", result.Name);
        }

        [Fact]
        public async Task CreateTvShowAsync_ShouldCreateAndReturnTvShow()
        {
            // Arrange
            var createDto = new CreateTvShowDto
            {
                Name = "New Show",
                Description = "New description",
                StartDate = DateTime.Now,
                Status = "Running",
                Network = "Netflix",
                Rating = 8.0,
                Genres = new List<string> { "Comedy" },
                ShowType = "Series"
            };

            var createdTvShow = new TvShow
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                Status = createDto.Status,
                Network = createDto.Network,
                Rating = createDto.Rating,
                Genres = createDto.Genres,
                ShowType = createDto.ShowType
            };

            _unitOfWorkMock.Setup(x => x.TvShows.AddAsync(It.IsAny<TvShow>()))
                .ReturnsAsync(createdTvShow);

            // Act
            var result = await _tvShowService.CreateTvShowAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Show", result.Name);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}