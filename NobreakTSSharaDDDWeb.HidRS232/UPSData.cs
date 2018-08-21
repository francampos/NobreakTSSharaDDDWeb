namespace NobreakTSSharaDDDweb.HidRS232
{
    using System;

    [Serializable()]
    public class UpsData
    {
        public bool BateriaBaixa;
        public double TensaoBateria = 0.0;
        public double PorcentagemBateria = 0.0;
        public bool BeepLigado = true;
        public bool StatusBypassBoostBuckActive;
        public double TensaoEntrada2 = 0.0;
        public double Frequencia = 0.0;
        public double TensaoEntrada = 0.0;
        public int OMaximumCurrent = 0;
        public double TensaoSaida = 0.0;
        public bool DesligamentoAtivo;
        public bool Success = false;
        public double Temperatura = 0.0; //TODO verificar se 0º é valido como valor default
        public bool TesteEmProgresso;
        public bool FalhaUps;
        public string UpsStatus = "";
        public bool UpsStandBy;
        public bool UtilityFailImmediate;
        public double Carga;

        public override string ToString()
        {
            return string.Format("Tensao de Entrada: {0}, Tensao de Saida: {1}, Bateria Baixa? {2}", TensaoEntrada, TensaoSaida, BateriaBaixa);
        }
    }
}

