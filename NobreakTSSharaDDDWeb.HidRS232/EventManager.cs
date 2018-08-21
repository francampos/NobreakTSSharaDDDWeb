using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NobreakTSSharaDDDWeb.HidRS232;

namespace NobreakTSSharaDDDweb.HidRS232
{
    /// <summary>
    /// Classe para tratar o disparo de eventos do Nobreak. 
    /// 
    /// Como o nobreak não dispara eventos para mundanças no estado, a consulta é feita constamente para descobrir o estado atual, informar eventuais mundanças.
    /// Por exemplo, a cada consulta é sempre encontrado o estado de Ok ou Falha de Rede para a rede elétrica.Mas deve disparar um evento apenas quando houver
    /// mudança neste estado.
    /// </summary>
    public class NobreakEventManager
    {
        //Variáveis para guardar o último evento disparado de cada tipo (Comunicação, Rede ou Bateria)
        private static EventHandler<NobreakEventArgs> lastEventComunicacao;
        private static EventHandler<NobreakEventArgs> lastEventRede;
        private static EventHandler<NobreakEventArgs> lastEventBateria;


        //TODO: Procurar forma de eliminar repetição
        public static void FireEventComunicacao(EventHandler<NobreakEventArgs> eventHandler, object sender, NobreakEventArgs e)
        {
            if (lastEventComunicacao == null || eventHandler.Method != lastEventComunicacao.Method) //TODO: Melhorar metodo de comparacao (criar EventHandler mais específico)
            {
                eventHandler.Invoke(sender, e);
                lastEventComunicacao = eventHandler;
            }
        }

        public static void FireEventRede(EventHandler<NobreakEventArgs> eventHandler, object sender, NobreakEventArgs e)
        {
            if (lastEventRede == null || eventHandler.Method.Name != lastEventRede.Method.Name)
            {
                eventHandler.Invoke(sender, e);
                lastEventRede = eventHandler;
            }
        }

        public static void FireEventBateria(EventHandler<NobreakEventArgs> eventHandler, object sender, NobreakEventArgs e)
        {
            if (lastEventBateria == null || eventHandler.Method != lastEventBateria.Method)
            {
                eventHandler.Invoke(sender, e);
                lastEventBateria = eventHandler;
            }
        }
    }


    //public delegate void BateriaBaixaEventHandler(object sender, EventArgs e);
    //public delegate void ComunicacaoOkEventHandler(object sender, EventArgs e);
    //public delegate void FalhaDeRedeEventHandler(object sender, EventArgs e);
    //public delegate void RedeOkEventHandler(object sender, EventArgs e);
    //public delegate void FalhaComunicacaoEventHandler(object sender, EventArgs e);
    //public delegate void AnormalidadeEventHandler(object sender, EventArgs e);

}
