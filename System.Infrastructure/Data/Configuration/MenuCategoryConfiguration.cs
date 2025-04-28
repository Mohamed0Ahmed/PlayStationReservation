using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
    {
        public void Configure(EntityTypeBuilder<MenuCategory> builder)
        {
            builder.Property(mc => mc.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(mc => mc.Store)
                .WithMany(s => s.MenuCategories)
                .HasForeignKey(mc => mc.StoreId);

            builder.HasIndex(mc => new { mc.Name, mc.StoreId })
                .IsUnique();
        }
    }
}