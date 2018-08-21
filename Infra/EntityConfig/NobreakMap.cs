using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Infra.EntityConfig
{
    public class NobreakMap : IEntityTypeConfiguration<Nobreak>
    {

        public void Configure(EntityTypeBuilder<Nobreak> builder)
        {
            builder.Property(e => e.Serial)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(e => e.CompanyName)
                .HasColumnType("varchar(90)")
                .IsRequired();

            builder.Property(e => e.UpsModel)
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.Property(e => e.Version)
                .HasColumnType("varchar(10")
                .IsRequired();

            builder.Property(e => e.Label)
                .HasColumnType("varchar(20)")
                .IsRequired();
        }
    }
}
