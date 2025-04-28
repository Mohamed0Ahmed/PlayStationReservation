using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.Property(mi => mi.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mi => mi.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(mi => mi.PointsRequired)
                .IsRequired();


            builder.HasOne(mi => mi.MenuCategory)
                .WithMany(mc => mc.MenuItems)
                .HasForeignKey(mi => mi.MenuCategoryId);


            builder.HasIndex(mi => new { mi.Name, mi.MenuCategoryId })
                .IsUnique();
        }
    }
}