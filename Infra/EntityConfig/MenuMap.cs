using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Infra.EntityConfig
{
    public class MenuMap : IEntityTypeConfiguration<Menu>
    {

        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder
                .HasMany(x => x.SubMenu)
                .WithOne()
                .HasForeignKey(x => x.MenuId);
        }

    }
}
