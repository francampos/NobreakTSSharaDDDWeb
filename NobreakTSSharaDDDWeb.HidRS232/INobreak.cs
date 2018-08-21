namespace NobreakTSSharaDDDweb.HidRS232
{
    public interface IUpsDevice
    {
        UpsData UpsInquiry();

        bool Conectado();

        StatusInfo Status();

        InformationData InformationInquiry();
        
    }
}
