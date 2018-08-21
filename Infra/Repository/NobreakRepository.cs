using System.Collections.Generic;
using System.Linq;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface.Services;
using NobreakTSSharaDDDWeb.Infra.Data;

namespace NobreakTSSharaDDDWeb.Infra.Repository
{
    public class NobreakRepository : EfDbRepository<Nobreak>, INobreakService
    {
        public NobreakRepository(NobreakContext dbContext) : base(dbContext)
        {
                
        }

        public IEnumerable<Nobreak> BuscarPorNome(string upsModel)
        {
            yield return Buscar(x => x.Nobreaks.Any(p => p.UpsModel == upsModel))
                            .FirstOrDefault();
        }
    }
}
