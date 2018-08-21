using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Infra.EntityConfig
{
    public class NobreakDemandDataMap : IEntityTypeConfiguration<NobreakDemandData>
    {

        public void Configure(EntityTypeBuilder<NobreakDemandData> builder)
        {

        }

        //public NobreakDemandDataConfigurations()
        //{
            //ToTable("NobreakData");
            //HasKey(x => x.Id);
            //Property(x => x.Id).HasColumnName("Id");
            //Property(x => x.EventReasons).HasColumnName("Event").IsOptional();
            //Property(x => x.InputVoltage).HasColumnName("InputVoltage").IsOptional();
            //Property(x => x.OutputVoltage).HasColumnName("OutputVoltage").IsOptional();
            //Property(x => x.Load).HasColumnName("Load").IsOptional();
            //Property(x => x.Battery).HasColumnName("Battery").IsOptional();
            //Property(x => x.Frequency).HasColumnName("Frequency").IsOptional();
            //Property(x => x.Temperature).HasColumnName("Temperature").IsOptional();
            //Property(x => x.CreationData).HasColumnName("CreationDate").IsOptional();
        //}

    }
}
