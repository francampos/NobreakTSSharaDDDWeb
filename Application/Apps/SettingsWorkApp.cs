using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface.Repository;
using NobreakTSSharaDDDWeb.Infra.Repository;

namespace NobreakTSSharaDDDWeb.Application.Apps
{
    public class SettingsWorkApp
    {
        private readonly IRepository<SettingsWork> _repository = null;

        public SettingsWorkApp()
        {
            _repository = new SettingsWorkRepository();
        }

        public void Create(SettingsWork settingsWork)
        {
            _repository.Adicionar(settingsWork);
        }

        public void Update(SettingsWork settingsWork)
        {
            _repository.Atualizar(settingsWork);
        }

        public void Delete(SettingsWork settingsWork)
        {
            _repository.Remover(settingsWork);
        }

    }
}
