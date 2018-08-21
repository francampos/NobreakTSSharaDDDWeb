using NobreakTSSharaDDDWeb.Domain.Entities;
using System.Collections.Generic;

namespace NobreakTSSharaDDDWeb.Application.Interfaces
{
    public interface INobreakDemandDataApp
    {
        void Add(NobreakDemandData obj);
        void AddOrUpdate(NobreakDemandData obj);
        NobreakDemandData FindById(int id);
        ICollection<NobreakDemandData> FindAll();
        void DeleteAll(IEnumerable<NobreakDemandData> obj);
        void Delete(NobreakDemandData obj);
        void Delete(int id);
        NobreakDemandData First();
        void Update(NobreakDemandData obj);
        void Commit();
        void Dispose();
    }
}
