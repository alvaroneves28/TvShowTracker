using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for the <see cref="UserFavorite"/> entity.
    /// Defines composite primary key, properties, relationships, indexes, and table mapping.
    /// </summary>
    public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
    {
        /// <summary>
        /// Configures the <see cref="UserFavorite"/> entity.
        /// </summary>
        /// <param name="builder">Entity type builder for <see cref="UserFavorite"/>.</param>
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            // Composite Primary Key (UserId + TvShowId)
            builder.HasKey(x => new { x.UserId, x.TvShowId });

            // Properties
            builder.Property(x => x.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TvShow)
                .WithMany(x => x.UserFavorites)
                .HasForeignKey(x => x.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for queries by User
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_UserFavorite_UserId");

            // Index for queries by TvShow
            builder.HasIndex(x => x.TvShowId)
                .HasDatabaseName("IX_UserFavorite_TvShowId");

            // Table name mapping
            builder.ToTable("UserFavorites");
        }
    }
}
