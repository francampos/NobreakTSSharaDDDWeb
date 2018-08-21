namespace NobreakTSSharaDDDweb.HidRS232
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Informações do modo de operação do Nobreak
    /// </summary>
    [Serializable()]
    public class StatusInfo
    {        
        [Description("Conectado")]
        public bool Conectado;

        [Description("Rede")]
        public StatusRede Rede;

        [Description("Bateria")]
        public StatusBateria Bateria;
    }
}

