using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework Core database context for the TvShowTracker application.
    /// Provides access to entity sets and applies entity configurations.
    /// </summary>
    public class TvShowContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TvShowContext"/> class.
        /// </summary>
        /// <param name="options">The options to configure the DbContext.</param>
        public TvShowContext(DbContextOptions<TvShowContext> options) : base(options) { }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="TvShow"/> entities.
        /// </summary>
        public DbSet<TvShow> TvShows { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="Episode"/> entities.
        /// </summary>
        public DbSet<Episode> Episodes { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="Actor"/> entities.
        /// </summary>
        public DbSet<Actor> Actors { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="User"/> entities.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for <see cref="UserFavorite"/> entities.
        /// </summary>
        public DbSet<UserFavorite> UserFavorites { get; set; }

        /// <summary>
        /// Configures the model by applying entity configurations.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all IEntityTypeConfiguration classes from this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowContext).Assembly);
        }
    }
}
