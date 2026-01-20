using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class MovieConfig : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Director)
            .HasMaxLength(200)
            .IsRequired();

        // Старий варіант: Зберігання акторів просто строкою
        builder.Property(x => x.Actors)
           .HasMaxLength(500);

        // // Новий варіант: Зв'язок Many-to-Many з сутністю Actor
        // builder.HasMany(x => x.Actors)
        //     .WithMany(x => x.Movies)
        //     .UsingEntity(j => j.ToTable("MovieActors"));

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.Property(x => x.AgeRestriction)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnType("decimal(4,2)");

        builder.Property(x => x.Genres)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PosterUrl)
            .HasMaxLength(500);

        builder.Property(x => x.TrailerUrl)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);
    }
}