using System.ComponentModel.DataAnnotations;

namespace NobreakTSSharaDDDWeb.Domain.Enuns
{
    public enum EventReasons
    {
        [Display(Name = "Rede Ok")]
        RedeOk,

        [Display(Name = "Bateria Baixa")]
        BateriaBaixa,

        [Display(Name = "Comunicação OK")]
        ComunicacaoOk,

        [Display(Name = "Falha de Rede")]
        FalhaRede,

        [Display(Name = "Falha de Comunicação")]
        FalhaComunicacao,

        [Display(Name = "Anormalidade")]
        Anormalidade,

        [Display(Name = "Bateria OK")]
        BateriaOk,

        [Display(Name = "Teste Solicitado")]
        TesteSolicitado,

        RetornoDeTeste,
        HibernacaoSistema,
        DesligamentoSistema,
        RetornoSistema
    }
}

