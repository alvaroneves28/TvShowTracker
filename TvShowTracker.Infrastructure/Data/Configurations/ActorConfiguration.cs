using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for the <see cref="Actor"/> entity.
    /// Defines primary key, properties, indexes, and relationships.
    /// </summary>
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        /// <summary>
        /// Configures the <see cref="Actor"/> entity.
        /// </summary>
        /// <param name="builder">Entity type builder for <see cref="Actor"/>.</param>
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Character)
                .HasMaxLength(150);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            // Index for faster search by Name
            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Actor_Name");

            // Many-to-Many relationship with TvShow
            builder.HasMany(x => x.TvShows)
                .WithMany(x => x.Actors)
                .UsingEntity<Dictionary<string, object>>(
                    "TvShowActors", // Join table name
                    j => j.HasOne<TvShow>().WithMany().HasForeignKey("TvShowId"),
                    j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                    j =>
                    {
                        // Composite Primary Key for join table
                        j.HasKey("ActorId", "TvShowId");

                        // Indexes for join table
                        j.HasIndex("TvShowId").HasDatabaseName("IX_TvShowActors_TvShowId");
                        j.HasIndex("ActorId").HasDatabaseName("IX_TvShowActors_ActorId");
                    });

            // Table name mapping
            builder.ToTable("Actors");
        }
    }
}
