using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface;
using NobreakTSSharaDDDWeb.Infra.Data;

namespace NobreakTSSharaDDDWeb.Infra.Repository
{
    public class SettingsWorkRepository : EfDbRepository<SettingsWork>, ISettingsWorkRepository
    {

        public SettingsWorkRepository(NobreakContext dbContext) : base(dbContext)
        {
        }
    }
}
