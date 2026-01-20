using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TicketConfig : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        // Composite key
        builder.HasKey(x => new { x.OrderId, x.SessionId, x.SeatId });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Tickets)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.Seat)
            .WithMany()
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}