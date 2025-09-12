using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for the <see cref="User"/> entity.
    /// Defines primary key, properties, indexes, relationships, and table mapping.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the <see cref="User"/> entity.
        /// </summary>
        /// <param name="builder">Entity type builder for <see cref="User"/>.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

            // Indexes for performance and uniqueness
            builder.HasIndex(x => x.Username)
                .IsUnique()
                .HasDatabaseName("IX_User_Username");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("IX_User_Email");

            // Relationships
            builder.HasMany(x => x.Favorites)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table name mapping
            builder.ToTable("Users");
        }
    }
}
