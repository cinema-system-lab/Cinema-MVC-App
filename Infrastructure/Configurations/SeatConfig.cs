using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SeatConfig : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RowNumber)
            .IsRequired();

        builder.Property(x => x.SeatNumber)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<byte>()
            .IsRequired();

        builder.HasOne(x => x.Hall)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.HallId);

        // Один і той самий номер місця в одному ряду — унікальний
        builder.HasIndex(x => new { x.HallId, x.RowNumber, x.SeatNumber })
            .IsUnique();
    }
}