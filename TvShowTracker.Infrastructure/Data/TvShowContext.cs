using Microsoft.EntityFrameworkCore;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data
{
    public class TvShowContext : DbContext
    {
        public TvShowContext(DbContextOptions<TvShowContext> options) : base(options) { }

        public DbSet<TvShow> TvShows { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvShowContext).Assembly);
        }
    }
}