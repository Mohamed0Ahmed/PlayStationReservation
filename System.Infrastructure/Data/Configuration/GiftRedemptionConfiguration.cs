using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class GiftRedemptionConfiguration : IEntityTypeConfiguration<GiftRedemption>
    {
        public void Configure(EntityTypeBuilder<GiftRedemption> builder)
        {
            builder.HasKey(gr => gr.Id);

            builder.Property(gr => gr.Status)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(gr => gr.RejectionReason)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne<Gift>()
                   .WithMany()
                   .HasForeignKey(gr => gr.GiftId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(gr => gr.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
