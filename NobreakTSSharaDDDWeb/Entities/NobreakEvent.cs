using System;
using System.ComponentModel.DataAnnotations;
using NobreakTSSharaDDDWeb.Domain.Enuns;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class NobreakEvent
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Evento")]
        public EventReasons EventReasons { get; set; }
        [Display(Name = "Tensão de Entrada")]
        public double InputVoltage { get; set; }
        [Display(Name = "Tensão de Saída")]
        public double OutputVoltage { get; set; }
        [Display(Name = "Carga")]
        public double Load { get; set; }
        [Display(Name = "Bateria")]
        public double Battery { get; set; }
        [Display(Name = "Frequência")]
        public double Frequency { get; set; }
        [Display(Name = "Temperatura")]
        public double Temperature { get; set; }
        [Display(Name = "Registrado em")]
        public DateTime CreationData { get; set; }

        //[Required]
        //public virtual TEntity Nobreak { get; set; }
    }
}
