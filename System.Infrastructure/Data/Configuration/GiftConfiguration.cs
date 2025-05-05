using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class GiftConfiguration : IEntityTypeConfiguration<Gift>
    {
        public void Configure(EntityTypeBuilder<Gift> builder)
        {
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.PointsRequired)
                .IsRequired();

            builder.HasOne<Store>()
                .WithMany(s => s.Gifts)
                .HasForeignKey(g => g.StoreId);

            builder.HasIndex(g => new { g.Name, g.StoreId })
                .IsUnique();
        }
    }
}