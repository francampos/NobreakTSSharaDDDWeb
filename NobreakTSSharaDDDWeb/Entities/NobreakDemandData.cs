using System;
using NobreakTSSharaDDDWeb.Domain.Enuns;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class NobreakDemandData
    {
        public NobreakDemandData()
        {
            CreationData = DateTime.Now;
        }

        public int Id { get; set; }
        public TechnicalData NobreakTechnicalData { get; set; }
        public EventReasons EventReasons { get; set; }
        public double InputVoltage { get; set; }
        public double OutputVoltage { get; set; }
        public double Load { get; set; }
        public double Battery { get; set; }
        public double Frequency { get; set; }
        public double Temperature { get; set; }
        public DateTime CreationData { get; set; }
    }
}
