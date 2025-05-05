using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class PointSettingConfiguration : IEntityTypeConfiguration<PointSetting>
    {
        public void Configure(EntityTypeBuilder<PointSetting> builder)
        {
            builder.Property(ps => ps.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(ps => ps.Points)
                .IsRequired();

            builder.HasOne<Store>()
                .WithMany(s => s.PointSettings)
                .HasForeignKey(ps => ps.StoreId);
        }
    }
}