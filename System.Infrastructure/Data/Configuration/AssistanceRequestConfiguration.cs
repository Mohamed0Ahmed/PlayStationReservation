using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class AssistanceRequestConfiguration : IEntityTypeConfiguration<AssistanceRequest>
    {
        public void Configure(EntityTypeBuilder<AssistanceRequest> builder)
        {
            builder.Property(ar => ar.RequestType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ar => ar.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ar => ar.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(ar => ar.Customer)
                .WithMany(c => c.AssistanceRequests)
                .HasForeignKey(ar => ar.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(ar => ar.Room)
                .WithMany(r => r.AssistanceRequests)
                .HasForeignKey(ar => ar.RoomId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}