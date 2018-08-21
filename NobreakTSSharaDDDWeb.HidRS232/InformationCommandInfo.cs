using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NobreakTSSharaDDDweb.HidRS232
{
    [Serializable()]
    public class InformationData
    {
        public string Fabricante = "";
        public bool Success = false;
        public string Modelo = "";
        public string Versao = "";
    }
}
