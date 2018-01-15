using GDA;
using System.ComponentModel;

namespace Glass.Data.Model
{
    public enum SituacaoArquivoQuitacaoParcelaCartao
    {
        [Description("Ativo")]
        Ativo = 1,

        [Description("Cancelado")]
        Cancelado
    }

    [PersistenceClass("arquivo_quitacao_parcela_cartao")]
    public class ArquivoQuitacaoParcelaCartao : ModelBaseCadastro
    {
        [PersistenceProperty("IDARQUIVOQUITACAOPARCELACARTAO", PersistenceParameterType.IdentityKey)]
        public int IdArquivoQuitacaoParcelaCartao { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoArquivoQuitacaoParcelaCartao Situacao { get; set; }
    }
}
