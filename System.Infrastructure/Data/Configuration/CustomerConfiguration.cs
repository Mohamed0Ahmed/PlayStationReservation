using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Points)
                .HasDefaultValue(0);

            builder.HasOne<Store>()
                .WithMany(s => s.Customers)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(c => new { c.PhoneNumber, c.StoreId })
                .IsUnique();
        }
    }
}