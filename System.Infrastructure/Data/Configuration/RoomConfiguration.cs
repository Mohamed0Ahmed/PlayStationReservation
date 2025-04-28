using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(r => r.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Password)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(r => r.Store)
                .WithMany(s => s.Rooms)
                .HasForeignKey(r => r.StoreId);

            builder.HasIndex(r => new { r.Username, r.StoreId })
                .IsUnique();
        }
    }
}