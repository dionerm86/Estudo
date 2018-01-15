using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;
using System.ComponentModel;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    public enum SituacaoArquivoCartaoNaoIdentificado
    {
        [Description("Ativo")]
        Ativo = 1,

        [Description("Cancelado")]
        Cancelado
    }

    [PersistenceClass("arquivo_cartao_nao_identificado")]
    public class ArquivoCartaoNaoIdentificado : ModelBaseCadastro
    {
        [Log(TipoLog.Cancelamento, "Arquivo cartão não identificado")]
        [PersistenceProperty("IDARQUIVOCARTAONAOIDENTIFICADO", PersistenceParameterType.IdentityKey)]
        public int IdArquivoCartaoNaoIdentificado { get; set; }
        
        [PersistenceProperty("SITUACAO")]
        public SituacaoArquivoCartaoNaoIdentificado Situacao { get; set; }
    }
}
