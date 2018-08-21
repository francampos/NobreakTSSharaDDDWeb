using System;
using System.Collections.Generic;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Domain.Interface.Services
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INobreakServiceCallbackChannel))]
    public interface INobreakServiceChannel
    {
        [OperationContract]
        UpsData UpsInquiry();

        [OperationContract]
        InformationData InformationInquiry();

        [OperationContract]
        StatusInfo Status();

        [OperationContract]
        void TesteFor10Seconds();

        [OperationContract(IsOneWay = true)]
        void Connect();

        [OperationContract(IsOneWay = false)]
        bool DisplayMessage(String message);

        [OperationContract(IsOneWay = true)]
        void ToggleBeep();

        [OperationContract]
        void TestForMinutes(int min);

        [OperationContract]
        void TestUntilBatteryLow();

        [OperationContract]
        void CancelTests();
        
        ICollection<NobreakDemandData> AllEvents();
    }
}
