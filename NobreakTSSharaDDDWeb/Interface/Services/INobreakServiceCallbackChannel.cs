using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Domain.Interface.Services
{
    [ServiceContract]
    public interface INobreakServiceCallbackChannel
    {
        [OperationContract(IsOneWay = true)]
        void UpdateStatus(StatusUpdate status);

        [OperationContract(IsOneWay = true)]
        void ServiceShutdown();


    }
}
