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
    public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            // Composite Primary Key
            builder.HasKey(x => new { x.UserId, x.TvShowId });

            // Properties
            builder.Property(x => x.AddedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.TvShow)
                .WithMany(x => x.UserFavorites)
                .HasForeignKey(x => x.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index para queries por usuário
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_UserFavorite_UserId");

            // Index para queries por série
            builder.HasIndex(x => x.TvShowId)
                .HasDatabaseName("IX_UserFavorite_TvShowId");

            // Table name
            builder.ToTable("UserFavorites");
        }
    }
}
