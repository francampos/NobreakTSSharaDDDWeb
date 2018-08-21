using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class Nobreak
    {
        public Nobreak()
        {

        }

        [Key]
        public int Id { get; set; }
        [Required]
        //[Index(IsUnique = true)]
        [StringLength(50)]
        public string Serial { get; set; }
        public string CompanyName { get; set; }
        public string UpsModel { get; set; }
        public string Version { get; set; }
        public string Label { get; set; } 

        [JsonIgnore]
        public virtual ICollection<Nobreak> Nobreaks { get; set; }

        [JsonIgnore]
        public virtual ICollection<NobreakEvent> NobreakEvents { get; set; }

        public override string ToString()
        {
            return Serial;
        }
    }
}
