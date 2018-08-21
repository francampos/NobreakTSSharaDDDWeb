using System.Collections.Generic;

namespace NobreakTSSharaDDDWeb.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int? MenuId { get; set; }

        public ICollection<Menu> SubMenu { get; set; }
      
    }
}
