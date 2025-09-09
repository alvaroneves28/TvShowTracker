using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowTracker.Core.Entities;

namespace TvShowTracker.Infrastructure.Data.Configurations
{
    public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Season)
                .IsRequired();

            builder.Property(x => x.EpisodeNumber)
                .IsRequired();

            builder.Property(x => x.AirDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.Summary)
                .HasMaxLength(2000);

            builder.Property(x => x.Rating)
                .HasColumnType("decimal(3,1)") // Ex: 8.5
                .HasDefaultValue(0.0);

            builder.Property(x => x.TvShowId)
                .IsRequired();

            // Indexes para performance e queries comuns
            builder.HasIndex(x => x.TvShowId)
                .HasDatabaseName("IX_Episode_TvShowId");

            builder.HasIndex(x => new { x.TvShowId, x.Season, x.EpisodeNumber })
                .IsUnique()
                .HasDatabaseName("IX_Episode_TvShow_Season_Episode");

            builder.HasIndex(x => x.AirDate)
                .HasDatabaseName("IX_Episode_AirDate");

            // Relationships
            builder.HasOne(x => x.TvShow)
                .WithMany(x => x.Episodes)
                .HasForeignKey(x => x.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table name
            builder.ToTable("Episodes");

            // Check constraints para validação a nível de BD
            builder.HasCheckConstraint("CK_Episode_Season", "[Season] > 0");
            builder.HasCheckConstraint("CK_Episode_EpisodeNumber", "[EpisodeNumber] > 0");
            builder.HasCheckConstraint("CK_Episode_Rating", "[Rating] >= 0 AND [Rating] <= 10");
        }
    }
}
