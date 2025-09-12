using TvShowTracker.Application.DTOs;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Tests.Helpers
{
    /// <summary>
    /// Provides helper methods to create test data for unit and integration tests.
    /// </summary>
    public static class TestDataBuilder
    {
        /// <summary>
        /// Creates a <see cref="TvShow"/> instance with default or specified values.
        /// </summary>
        public static TvShow CreateTvShow(
            int id = 1,
            string name = "Test Show",
            string status = "Running",
            double rating = 8.0)
        {
            return new TvShow
            {
                Id = id,
                Name = name,
                Description = $"Description for {name}",
                StartDate = DateTime.Now.AddYears(-1),
                Status = status,
                Network = "Test Network",
                ImageUrl = "https://example.com/image.jpg",
                Rating = rating,
                Genres = new List<string> { "Drama", "Action" },
                ShowType = "Series",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Episodes = new List<Episode>(),
                Actors = new List<Actor>()
            };
        }

        /// <summary>
        /// Creates an <see cref="Episode"/> instance with default or specified values.
        /// </summary>
        public static Episode CreateEpisode(
            int id = 1,
            int tvShowId = 1,
            string name = "Test Episode",
            int season = 1,
            int episodeNumber = 1)
        {
            return new Episode
            {
                Id = id,
                TvShowId = tvShowId,
                Name = name,
                Season = season,
                EpisodeNumber = episodeNumber,
                AirDate = DateTime.Now.AddDays(-30),
                Summary = $"Summary for {name}",
                Rating = 8.0
            };
        }

        /// <summary>
        /// Creates an <see cref="Actor"/> instance with default or specified values.
        /// </summary>
        public static Actor CreateActor(
            int id = 1,
            string name = "Test Actor",
            string character = "Test Character")
        {
            return new Actor
            {
                Id = id,
                Name = name,
                Character = character,
                ImageUrl = "https://example.com/actor.jpg",
                TvShows = new List<TvShow>()
            };
        }

        /// <summary>
        /// Creates a <see cref="User"/> instance with default or specified values.
        /// </summary>
        public static User CreateUser(
            int id = 1,
            string username = "testuser",
            string email = "test@example.com")
        {
            return new User
            {
                Id = id,
                Username = username,
                Email = email,
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                Favorites = new List<UserFavorite>()
            };
        }

        /// <summary>
        /// Creates a <see cref="CreateTvShowDto"/> instance with default or specified values.
        /// </summary>
        public static CreateTvShowDto CreateTvShowDto(
            string name = "New Test Show",
            string status = "Running")
        {
            return new CreateTvShowDto
            {
                Name = name,
                Description = $"Description for {name}",
                StartDate = DateTime.Now,
                Status = status,
                Network = "Test Network",
                ImageUrl = "https://example.com/image.jpg",
                Rating = 8.0,
                Genres = new List<string> { "Drama" },
                ShowType = "Series"
            };
        }

        /// <summary>
        /// Creates a <see cref="RegisterDto"/> instance with default or specified values.
        /// </summary>
        public static RegisterDto CreateRegisterDto(
            string username = "newuser",
            string email = "newuser@example.com",
            string password = "Password123!")
        {
            return new RegisterDto
            {
                Username = username,
                Email = email,
                Password = password,
                ConfirmPassword = password
            };
        }

        /// <summary>
        /// Creates a <see cref="LoginDto"/> instance with default or specified values.
        /// </summary>
        public static LoginDto CreateLoginDto(
            string username = "testuser",
            string password = "Password123!")
        {
            return new LoginDto
            {
                Username = username,
                Password = password
            };
        }
    }
}
