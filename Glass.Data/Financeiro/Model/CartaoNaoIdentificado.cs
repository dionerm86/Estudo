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
    public enum SituacaoCartaoNaoIdentificado
    {
        [Description("Ativo")]
        Ativo = 1,

        [Description("Cancelado")]
        Cancelado,

        [Description("Em Uso")]
        EmUso
    }

    [PersistenceBaseDAO(typeof(CartaoNaoIdentificadoDAO))]
    [PersistenceClass("cartao_nao_identificado")]
    public class CartaoNaoIdentificado : ModelBaseCadastro
    {
        [Log(TipoLog.Cancelamento, "Cartão não identificado")]
        [PersistenceProperty("IDCARTAONAOIDENTIFICADO", PersistenceParameterType.IdentityKey)]
        public int IdCartaoNaoIdentificado { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public int IdContaBanco { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }      

        [PersistenceProperty("TIPOCARTAO")]
        public int TipoCartao { get; set; }

        [PersistenceProperty("IDACERTO")]
        public int? IdAcerto { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public int? IdContaR { get; set; }

        [PersistenceProperty("IDOBRA")]
        public int? IdObra { get; set; }

        [PersistenceProperty("IDSINAL")]
        public int? IdSinal { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public int? IdTrocaDevolucao { get; set; }

        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public int? IdDevolucaoPagto { get; set; }

        [PersistenceProperty("IDACERTOCHEQUE")]
        public int? IdAcertoCheque { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public int? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public int? IdLiberarPedido { get; set; }
        
        [PersistenceProperty("SITUACAO")]
        public SituacaoCartaoNaoIdentificado Situacao { get; set; }

        [PersistenceProperty("OBS")]
        public string Observacao { get; set; }

        [PersistenceProperty("NumAutCartao")]
        public string NumAutCartao { get; set; }

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("IMPORTADO")]
        public bool Importado { get; set; }

        [PersistenceProperty("NUMEROPARCELAS")]
        public int NumeroParcelas { get; set; }

        [PersistenceProperty("DATAVENDA")]
        public DateTime DataVenda { get; set; }

        [PersistenceProperty("NUMEROESTABELECIMENTO")]
        public string NumeroEstabelecimento { get; set; }

        [PersistenceProperty("ULTIMOSDIGITOSCARTAO")]
        public string UltimosDigitosCartao { get; set; }

        [PersistenceProperty("IDARQUIVOCARTAONAOIDENTIFICADO")]
        public int? IdArquivoCartaoNaoIdentificado { get; set; }
    }
}
