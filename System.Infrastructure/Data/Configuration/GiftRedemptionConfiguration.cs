using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class GiftRedemptionConfiguration : IEntityTypeConfiguration<GiftRedemption>
    {
        public void Configure(EntityTypeBuilder<GiftRedemption> builder)
        {
            builder.Property(gr => gr.RedemptionDate)
                .IsRequired();

            builder.Property(gr => gr.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(gr => gr.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(gr => gr.Customer)
                .WithMany(c => c.GiftRedemptions)
                .HasForeignKey(gr => gr.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(gr => gr.Gift)
                .WithMany(g => g.GiftRedemptions)
                .HasForeignKey(gr => gr.GiftId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}