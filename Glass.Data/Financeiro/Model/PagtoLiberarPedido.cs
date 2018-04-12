using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoLiberarPedidoDAO))]
    [PersistenceClass("pagto_liberar_pedido")]
    public class PagtoLiberarPedido
    {
        #region Propriedades

        [PersistenceProperty("IDLIBERARPEDIDO", PersistenceParameterType.Key)]
        public uint IdLiberarPedido { get; set; }

        [PersistenceProperty("NUMFORMAPAGTO", PersistenceParameterType.Key)]
        public int NumFormaPagto { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public int? IdContaBanco { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public int? IdCartaoNaoIdentificado { get; set; }

        [PersistenceProperty("IDDEPOSITONAOIDENTIFICADO")]
        public int? IdDepositoNaoIdentificado { get; set; }

        [PersistenceProperty("VALORPAGTO", PersistenceParameterType.Key)]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("QUANTIDADEPARCELACARTAO")]
        public int? QuantidadeParcelaCartao { get; set; }

        [PersistenceProperty("NUMAUTCARTAO")]
        public string NumAutCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRFORMAPAGTO", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        #endregion
    }
}