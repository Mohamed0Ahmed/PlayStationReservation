using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.Room)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}