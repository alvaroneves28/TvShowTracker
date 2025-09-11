using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.API.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(TvShowContext context)
        {
            await context.Database.EnsureCreatedAsync();

            // Se já existem dados, não fazer seed
            if (await context.TvShows.AnyAsync())
                return;

            // Criar séries de teste
            var tvShows = new List<TvShow>
            {
                new TvShow
                {
                    Name = "Breaking Bad",
                    Description = "Um professor de química se torna fabricante de metanfetamina",
                    StartDate = new DateTime(2008, 1, 20),
                    Status = "Ended",
                    Network = "AMC",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
                    Rating = 9.5,
                    Genres = new List<string> { "Drama", "Crime", "Thriller" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TvShow
                {
                    Name = "Game of Thrones",
                    Description = "Luta pelo Trono de Ferro nos Sete Reinos de Westeros",
                    StartDate = new DateTime(2011, 4, 17),
                    Status = "Ended",
                    Network = "HBO",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/u3bZgnGQ9T01sWNhyveQy0S1m10.jpg",
                    Rating = 9.3,
                    Genres = new List<string> { "Drama", "Fantasy", "Action" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TvShow
                {
                    Name = "The Office",
                    Description = "Mockumentary sobre funcionários de escritório",
                    StartDate = new DateTime(2005, 3, 24),
                    Status = "Ended",
                    Network = "NBC",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/7DJKHzAi83BmQrWLrYYOqcoKfhR.jpg",
                    Rating = 8.8,
                    Genres = new List<string> { "Comedy", "Mockumentary" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new TvShow
                {
                    Name = "Stranger Things",
                    Description = "Eventos sobrenaturais em uma pequena cidade americana",
                    StartDate = new DateTime(2016, 7, 15),
                    Status = "Running",
                    Network = "Netflix",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/x2LSRK2Cm7MZhjluni1msVJ3wDF.jpg",
                    Rating = 8.7,
                    Genres = new List<string> { "Drama", "Fantasy", "Horror" },
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.TvShows.AddRange(tvShows);
            await context.SaveChangesAsync();

            // Criar episódios para Breaking Bad
            var breakingBad = tvShows.First(x => x.Name == "Breaking Bad");
            var episodes = new List<Episode>
            {
                new Episode
                {
                    TvShowId = breakingBad.Id,
                    Name = "Pilot",
                    Season = 1,
                    EpisodeNumber = 1,
                    AirDate = new DateTime(2008, 1, 20),
                    Summary = "Walter White, um professor de química, descobre que tem câncer terminal",
                    Rating = 8.2
                },
                new Episode
                {
                    TvShowId = breakingBad.Id,
                    Name = "Cat's in the Bag...",
                    Season = 1,
                    EpisodeNumber = 2,
                    AirDate = new DateTime(2008, 1, 27),
                    Summary = "Walter e Jesse tentam se livrar dos corpos",
                    Rating = 8.2
                },
                new Episode
                {
                    TvShowId = breakingBad.Id,
                    Name = "...And the Bag's in the River",
                    Season = 1,
                    EpisodeNumber = 3,
                    AirDate = new DateTime(2008, 2, 10),
                    Summary = "Walter enfrenta uma decisão difícil sobre Krazy-8",
                    Rating = 8.3
                }
            };

            context.Episodes.AddRange(episodes);

            // Criar atores
            var actors = new List<Actor>
            {
                new Actor
                {
                    Name = "Bryan Cranston",
                    Character = "Walter White",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/7Jahy5LZX2Fo8fGJltMReAI49hC.jpg"
                },
                new Actor
                {
                    Name = "Aaron Paul",
                    Character = "Jesse Pinkman",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/lOhc6GXlCLhBKDbxEyFPLI8Hv6A.jpg"
                },
                new Actor
                {
                    Name = "Anna Gunn",
                    Character = "Skyler White",
                    ImageUrl = "https://image.tmdb.org/t/p/w500/6yLKtfg0Ynx8CNsVmykZ2QWJqb0.jpg"
                }
            };

            context.Actors.AddRange(actors);
            await context.SaveChangesAsync();

            // Associar atores à série Breaking Bad
            breakingBad.Actors = actors;
            await context.SaveChangesAsync();

            Console.WriteLine("Dados de teste inseridos com sucesso!");
        }
    }
}
