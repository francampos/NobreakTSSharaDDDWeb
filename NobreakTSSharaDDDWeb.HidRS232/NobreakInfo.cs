using Microsoft.VisualBasic.CompilerServices;

namespace NobreakTSSharaDDDweb.HidRS232
{
    [StandardModule]
    internal sealed class NobreakInfo
    {
        public static bool StatusComunicacaoOk;
        public static int ContagemFalhaComunicacao = 0;
        public static double TensaoEntrada;
        public static double TensaoEntrada2;
        public static double TensaoSaida;
        public static long Carga;
        public static double Frequencia;
        public static double TensaoBateria;
        public static double TensaoSaidaNominal;
        public static long CargaNominal;
        public static double FrequenciaNominal;
        public static double TensaoBateriaNominal;
        public static bool StatusUtilityFail;
        public static bool StatusBateriaBaixa;
        public static bool StatusBypassBoostBuckActive;
        public static bool StatusFalhaUps;
        public static bool StatusUpsStandBy;
        public static bool StatusTesteEmProgresso;
        public static bool StatusShutdownAtivo;
        public static bool StatusBeepLigado;
        public static int PorcentagemBateria;
        public static double Temperatura;
        public static string FDados;
    }
}
