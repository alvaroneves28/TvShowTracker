using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for the <see cref="TvShow"/> entity.
    /// Defines primary key, properties, conversions, and table mapping.
    /// </summary>
    public class TvShowConfiguration : IEntityTypeConfiguration<TvShow>
    {
        /// <summary>
        /// Configures the <see cref="TvShow"/> entity.
        /// </summary>
        /// <param name="builder">Entity type builder for <see cref="TvShow"/>.</param>
        public void Configure(EntityTypeBuilder<TvShow> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Genres property with value conversion (stored as comma-separated string in DB)
            builder.Property(e => e.Genres)
                .HasConversion(
                    v => string.Join(',', v), // From List<string> to string
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() // From string to List<string>
                );

            // Other properties
            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Rating)
                .HasColumnType("decimal(3,1)");

            // Table name mapping
            builder.ToTable("TvShows");
        }
    }
}
