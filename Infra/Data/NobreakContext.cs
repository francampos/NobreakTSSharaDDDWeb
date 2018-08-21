using Microsoft.EntityFrameworkCore;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Infra.EntityConfig;

namespace NobreakTSSharaDDDWeb.Infra.Data
{
    public class NobreakContext : DbContext
    {
        public NobreakContext(DbContextOptions<NobreakContext> options) : base(options)
        {

        }

        public DbSet<Nobreak> Nobreaks { get; set; }
        public DbSet<NobreakEvent> NobreakEvents { get; set; }
        public DbSet<NobreakDemandData> NobreakDemandDatas { get; set; }
        public DbSet<SettingsWork> SettingsWorks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Nobreak>().ToTable("Nobreak");
            modelBuilder.Entity<NobreakEvent>().ToTable("NobreakEvent");
            modelBuilder.Entity<NobreakDemandData>().ToTable("NobreakDemandData");
            modelBuilder.Entity<SettingsWork>().ToTable("SettingsWork");
            modelBuilder.ApplyConfiguration(new NobreakMap());

        }
    }
}
