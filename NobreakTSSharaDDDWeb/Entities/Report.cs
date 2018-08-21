using System;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class Report
    {
        public String Evento { get; set; }
        public DateTime Horario { get; set; }
        public double VoltagemEntrada { get; set; }
        public double VoltagemSaida { get; set; }
        public double Carga { get; set; }
        public double Bateria { get; set; }
        public double Frequencia { get; set; }
        public double Temperatura { get; set; }
    }
}
