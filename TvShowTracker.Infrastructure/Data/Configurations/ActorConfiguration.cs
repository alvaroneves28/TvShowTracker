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
    public class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
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

            // Index para busca por nome
            builder.HasIndex(x => x.Name)
                .HasDatabaseName("IX_Actor_Name");

            // Many-to-Many relationship com TvShow
            builder.HasMany(x => x.TvShows)
                .WithMany(x => x.Actors)
                .UsingEntity<Dictionary<string, object>>(
                    "TvShowActors", // Nome da tabela de junção
                    j => j.HasOne<TvShow>().WithMany().HasForeignKey("TvShowId"),
                    j => j.HasOne<Actor>().WithMany().HasForeignKey("ActorId"),
                    j =>
                    {
                        j.HasKey("ActorId", "TvShowId");
                        j.HasIndex("TvShowId").HasDatabaseName("IX_TvShowActors_TvShowId");
                        j.HasIndex("ActorId").HasDatabaseName("IX_TvShowActors_ActorId");
                    });

            // Table name
            builder.ToTable("Actors");
        }
    }
}
