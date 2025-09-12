using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;
using TvShowTracker.Infrastructure.Data;

namespace TvShowTracker.API.Data
{
    /// <summary>
    /// Static utility class responsible for seeding the database with initial test data.
    /// Provides sample TV shows, episodes, and actors for development, testing, and demonstration purposes.
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Asynchronously seeds the database with initial test data including TV shows, episodes, and actors.
        /// This method ensures the database is created and populates it with sample data if it's empty.
        /// </summary>
        /// <param name="context">The database context used for data operations</param>
        /// <returns>A task representing the asynchronous seeding operation</returns>
        public static async Task SeedAsync(TvShowContext context)
        {
            // Ensure the database schema is created before attempting to seed data
            // This is particularly important for first-time application runs
            await context.Database.EnsureCreatedAsync();

            // Safety check: prevent duplicate seeding by checking for existing TV show data
            // This allows the seeder to be called multiple times without creating duplicates
            if (await context.TvShows.AnyAsync())
                return;

            // Create comprehensive sample TV show data representing different genres and networks
            // Each show includes realistic metadata that would be found in a production system
            var tvShows = new List<TvShow>
            {
                // Breaking Bad: Acclaimed drama series - representative of high-quality serialized drama
                new TvShow
                {
                    Name = "Breaking Bad",
                    Description = "A high school chemistry teacher turned methamphetamine manufacturer",
                    StartDate = new DateTime(2008, 1, 20), // Accurate premiere date
                    Status = "Ended", // Completed series status
                    Network = "AMC", // Original broadcasting network
                    ImageUrl = "https://image.tmdb.org/t/p/w500/ggFHVNu6YYI5L9pCfOacjizRGt.jpg", // Official poster image
                    Rating = 9.5, // Reflects actual critical acclaim
                    Genres = new List<string> { "Drama", "Crime", "Thriller" }, // Multi-genre classification
                    ShowType = "Series", // Standard episodic series format
                    CreatedAt = DateTime.UtcNow, // Audit timestamps
                    UpdatedAt = DateTime.UtcNow
                },
                
                // Game of Thrones: Epic fantasy series - representative of premium cable fantasy content
                new TvShow
                {
                    Name = "Game of Thrones",
                    Description = "The battle for the Iron Throne in the Seven Kingdoms of Westeros",
                    StartDate = new DateTime(2011, 4, 17), // Accurate premiere date
                    Status = "Ended", // Completed series
                    Network = "HBO", // Premium cable network
                    ImageUrl = "https://image.tmdb.org/t/p/w500/u3bZgnGQ9T01sWNhyveQy0S1m10.jpg", // Official artwork
                    Rating = 9.3, // High critical rating
                    Genres = new List<string> { "Drama", "Fantasy", "Action" }, // Genre diversity
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                
                // The Office: Workplace comedy - representative of mockumentary/comedy format
                new TvShow
                {
                    Name = "The Office",
                    Description = "Mockumentary about office employees at a paper company",
                    StartDate = new DateTime(2005, 3, 24), // US version premiere date
                    Status = "Ended", // Long-running completed comedy
                    Network = "NBC", // Major broadcast network
                    ImageUrl = "https://image.tmdb.org/t/p/w500/7DJKHzAi83BmQrWLrYYOqcoKfhR.jpg", // Iconic show poster
                    Rating = 8.8, // Strong comedy rating
                    Genres = new List<string> { "Comedy", "Mockumentary" }, // Specific comedy subgenre
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                
                // Stranger Things: Modern streaming hit - representative of Netflix original content
                new TvShow
                {
                    Name = "Stranger Things",
                    Description = "Supernatural events in a small American town in the 1980s",
                    StartDate = new DateTime(2016, 7, 15), // Netflix original premiere
                    Status = "Running", // Currently active series (as of seeding time)
                    Network = "Netflix", // Streaming platform original
                    ImageUrl = "https://image.tmdb.org/t/p/w500/x2LSRK2Cm7MZhjluni1msVJ3wDF.jpg", // Distinctive poster art
                    Rating = 8.7, // Strong contemporary rating
                    Genres = new List<string> { "Drama", "Fantasy", "Horror" }, // Multi-genre supernatural content
                    ShowType = "Series",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Batch insert TV shows for efficiency - single database transaction
            context.TvShows.AddRange(tvShows);
            await context.SaveChangesAsync(); // Commit shows first to generate IDs for foreign key relationships

            // Create detailed episode data for Breaking Bad as a comprehensive example
            // This demonstrates proper episode metadata structure and relationships
            var breakingBad = tvShows.First(x => x.Name == "Breaking Bad");
            var episodes = new List<Episode>
            {
                // Pilot episode: Series foundation with accurate metadata
                new Episode
                {
                    TvShowId = breakingBad.Id, // Foreign key relationship to parent show
                    Name = "Pilot", // Original episode title
                    Season = 1, // Season numbering starts at 1
                    EpisodeNumber = 1, // Episode numbering within season
                    AirDate = new DateTime(2008, 1, 20), // Accurate original air date
                    Summary = "Walter White, a chemistry teacher, discovers he has terminal cancer", // Concise episode summary
                    Rating = 8.2 // Episode-specific rating
                },
                
                // Second episode: Continuation of pilot story arc
                new Episode
                {
                    TvShowId = breakingBad.Id,
                    Name = "Cat's in the Bag...", // Part of two-part episode title
                    Season = 1,
                    EpisodeNumber = 2,
                    AirDate = new DateTime(2008, 1, 27), // Weekly episode schedule
                    Summary = "Walter and Jesse attempt to dispose of evidence", // Plot progression summary
                    Rating = 8.2
                },
                
                // Third episode: Early series development
                new Episode
                {
                    TvShowId = breakingBad.Id,
                    Name = "...And the Bag's in the River", // Completion of two-part title
                    Season = 1,
                    EpisodeNumber = 3,
                    AirDate = new DateTime(2008, 2, 10), // Consistent scheduling pattern
                    Summary = "Walter faces a difficult decision regarding Krazy-8", // Character conflict focus
                    Rating = 8.3 // Slight rating improvement showing series momentum
                }
            };

            // Batch insert episodes for the sample show
            context.Episodes.AddRange(episodes);

            // Create actor data with character associations for Breaking Bad cast
            // Represents main cast members with accurate real-world information
            var actors = new List<Actor>
            {
                // Lead actor: Bryan Cranston as Walter White
                new Actor
                {
                    Name = "Bryan Cranston", // Real actor name
                    Character = "Walter White", // Character portrayed in the show
                    ImageUrl = "https://image.tmdb.org/t/p/w500/7Jahy5LZX2Fo8fGJltMReAI49hC.jpg" // Professional headshot
                },
                
                // Co-lead: Aaron Paul as Jesse Pinkman
                new Actor
                {
                    Name = "Aaron Paul", // Real actor name
                    Character = "Jesse Pinkman", // Character name from show
                    ImageUrl = "https://image.tmdb.org/t/p/w500/lOhc6GXlCLhBKDbxEyFPLI8Hv6A.jpg" // Professional photo
                },
                
                // Supporting cast: Anna Gunn as Skyler White
                new Actor
                {
                    Name = "Anna Gunn", // Real actress name
                    Character = "Skyler White", // Character role in series
                    ImageUrl = "https://image.tmdb.org/t/p/w500/6yLKtfg0Ynx8CNsVmykZ2QWJqb0.jpg" // Official image
                }
            };

            // Batch insert actor records
            context.Actors.AddRange(actors);
            await context.SaveChangesAsync(); // Commit actors to generate IDs

            // Establish many-to-many relationship between Breaking Bad and its cast
            // This demonstrates proper entity relationship management in Entity Framework
            breakingBad.Actors = actors;
            await context.SaveChangesAsync(); // Final commit to establish all relationships

            // Provide console feedback for successful completion
            // Useful for development and deployment verification
            Console.WriteLine("Test data successfully inserted into database!");
        }
    }
}