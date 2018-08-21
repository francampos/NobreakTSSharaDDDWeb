using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface;
using NobreakTSSharaDDDWeb.Domain.Interface.Repository;
using NobreakTSSharaDDDWeb.Infra.Repository;

namespace NobreakTSSharaDDDWeb.Application.Apps
{
    public class NobreakDemandDataRepository
    {
        private readonly IRepository<NobreakDemandData> _repository = null;

        public NobreakDemandDataApp()
        {
            _repository = new NobreakDemandDataRepository();
        }

        public void Create(NobreakDemandData nobreakDemandData)
        {
            _repository.Adicionar(nobreakDemandData);
        }
        
        public void Update(NobreakDemandData nobreakDemandData)
        {
            _repository.Atualizar(nobreakDemandData);
        }

        public void Delete(NobreakDemandData nobreakDemandData)
        {
            _repository.Remover(nobreakDemandData);
        }

    }
}
