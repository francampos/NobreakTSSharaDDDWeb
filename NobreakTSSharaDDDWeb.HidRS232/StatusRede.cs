using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NobreakTSSharaDDDweb.HidRS232
{
    public enum StatusRede
    {

        [Description("Desconhecido")]
        Desconhecido,

        [Description("OK")]
        Ok,

        [Description("By Pass")]
        ModoByPass,

        [Description("Anormal")]
        Anormal,

        [Description("Falha")]
        Falha,

        [Description("Operando em Bateria")]
        OperandoEmBateria,
        
        [Description("Operando em Rede")]
        OperandoEmRede
    }
}
