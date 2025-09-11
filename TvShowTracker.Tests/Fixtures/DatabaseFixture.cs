using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public TvShowContext Context { get; private set; }

        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<TvShowContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new TvShowContext(options);
            Context.Database.EnsureCreated();
            SeedTestData();
        }

        private void SeedTestData()
        {
            var tvShows = new List<TvShow>
            {
                new TvShow
                {
                    Id = 1,
                    Name = "Breaking Bad",
                    Description = "Um professor de química se torna fabricante de metanfetamina",
                    StartDate = new DateTime(2008, 1, 20),
                    Status = "Ended",
                    Network = "AMC",
                    ImageUrl = "https://example.com/breaking-bad.jpg",
                    Rating = 9.5,
                    Genres = new List<string> { "Drama", "Crime", "Thriller" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TvShow
                {
                    Id = 2,
                    Name = "Game of Thrones",
                    Description = "Luta pelo Trono de Ferro nos Sete Reinos de Westeros",
                    StartDate = new DateTime(2011, 4, 17),
                    Status = "Ended",
                    Network = "HBO",
                    ImageUrl = "https://example.com/got.jpg",
                    Rating = 9.3,
                    Genres = new List<string> { "Drama", "Fantasy", "Action" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            var episodes = new List<Episode>
            {
                new Episode
                {
                    Id = 1,
                    TvShowId = 1,
                    Name = "Pilot",
                    Season = 1,
                    EpisodeNumber = 1,
                    AirDate = new DateTime(2008, 1, 20),
                    Summary = "Walter White descobre que tem câncer terminal",
                    Rating = 8.2
                },
                new Episode
                {
                    Id = 2,
                    TvShowId = 1,
                    Name = "Cat's in the Bag...",
                    Season = 1,
                    EpisodeNumber = 2,
                    AirDate = new DateTime(2008, 1, 27),
                    Summary = "Walter e Jesse tentam se livrar dos corpos",
                    Rating = 8.2
                }
            };

            var actors = new List<Actor>
            {
                new Actor
                {
                    Id = 1,
                    Name = "Bryan Cranston",
                    Character = "Walter White",
                    ImageUrl = "https://example.com/bryan.jpg"
                },
                new Actor
                {
                    Id = 2,
                    Name = "Aaron Paul",
                    Character = "Jesse Pinkman",
                    ImageUrl = "https://example.com/aaron.jpg"
                }
            };

            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "test@example.com",
                    PasswordHash = "hashedpassword",
                    CreatedAt = DateTime.UtcNow
                }
            };

            Context.TvShows.AddRange(tvShows);
            Context.Episodes.AddRange(episodes);
            Context.Actors.AddRange(actors);
            Context.Users.AddRange(users);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
