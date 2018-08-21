using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NobreakTSSharaDDDWeb.UI.Models
{
    public class Nobreak //: IValidatableObject
    {
        public Nobreak()
        {
            //TODO: Verificar melhor metodo para nao receber ponteiro nulo ao chamar NobreakEvents.Add
            NobreakEvents = new List<NobreakEvent>();
            Users = new List<ApplicationUser>();
        }

        [Key]
        public int ID { get; set; }
        [Required]
        //[Index(IsUnique = true)]
        [StringLength(50)]
        public string Serial { get; set; }
        public string CompanyName { get; set; }
        public string UpsModel { get; set; }
        public string Version { get; set; }
        public string Label { get; set; } //humanizar identificação do nobreak

        [JsonIgnore]
        public virtual ICollection<ApplicationUser> Users { get; set; }
        [JsonIgnore] 
        //[IgnoreDataMember]
        public virtual ICollection<NobreakEvent> NobreakEvents { get; set; }

        public override string ToString()
        {
            return Serial;
        }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    var db = new ApplicationDbContext();
        //    if (db.Nobreaks.Any(nb => nb.Serial.Equals(Serial) && nb.ID != 0))
        //    {
        //        yield return new ValidationResult("Serial ja em uso");
        //    }

        //}
    }
}