using NobreakTSSharaDDDWeb.Domain.Entities;
using System.Collections.Generic;

namespace NobreakTSSharaDDDWeb.Application.Interfaces
{
    public interface ISettingsWorkApp
    {
        void Add(SettingsWork obj);
        void AddOrUpdate(SettingsWork obj);
        SettingsWork FindById(int id);
        ICollection<SettingsWork> FindAll();
        void DeleteAll(IEnumerable<SettingsWork> obj);
        void Delete(SettingsWork obj);
        void Delete(int id);
        SettingsWork First();
        void Update(SettingsWork obj);
        void Commit();
        void Dispose();
    }
}
