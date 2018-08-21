using System.Collections.Generic;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface.Repository;

namespace NobreakTSSharaDDDWeb.Domain.Interface
{
    public interface INobreakRepository : IRepository<Nobreak>
    {
        IEnumerable<Nobreak> BuscarPorNome(string nome);
    }
}
