using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Infra.EntityConfig
{
    public class NobreakEventMap : IEntityTypeConfiguration<NobreakEvent>
    {

        public void Configure(EntityTypeBuilder<NobreakEvent> builder)
        {

        }
    }
}
