using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatBooking.Domain.ReservationAggregate;
using SeatBooking.Domain.SeatAggregate;

namespace SeatBooking.Infrastructure.Configurations;

internal sealed class ReservationConfiguration
    : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ReservationId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.SeatId)
            .HasConversion(
                id => id.Value,
                value => new SeatId(value))
            .IsRequired();

        builder.Property(x => x.ClientId)
            .HasConversion(
                id => id.Value,
                value => new ClientId(value))
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.SeatId, x.Status });
        builder.HasIndex(x => x.ClientId);

        builder.HasOne<Seat>()
            .WithMany()
            .HasForeignKey(x => x.SeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}