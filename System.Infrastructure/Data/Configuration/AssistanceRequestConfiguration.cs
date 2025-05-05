using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class AssistanceRequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.Property(ar => ar.RequestTypeId)
                .IsRequired();

            builder.Property(ar => ar.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ar => ar.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne<Customer>()
                .WithMany(c => c.AssistanceRequests)
                .HasForeignKey(ar => ar.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<Room>()
                .WithMany(r => r.AssistanceRequests)
                .HasForeignKey(ar => ar.RoomId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<AssistanceRequestType>()
                .WithMany()
                .HasForeignKey(ar => ar.RequestTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}