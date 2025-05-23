﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Domain.Models;

namespace System.Infrastructure.Data.Configuration
{
    public class AssistanceRequestTypeConfiguration : IEntityTypeConfiguration<AssistanceRequestType>
    {
        public void Configure(EntityTypeBuilder<AssistanceRequestType> builder)
        {
            builder.Property(art => art.Id)
                .UseIdentityColumn(1000, 1);


            builder.Property(art => art.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne<Store>()
                .WithMany(s => s.AssistanceRequestTypes)
                .HasForeignKey(art => art.StoreId);

            builder.HasIndex(art => new { art.Name, art.StoreId })
                .IsUnique();
        }
    }
}