using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoCreditoFornecedorDAO))]
    [PersistenceClass("pagto_credito_fornecedor")]
    public class PagtoCreditoFornecedor
    {
        #region Propriedades

        [PersistenceProperty("IdCreditoFornecedor", PersistenceParameterType.Key)]
        public uint IdCreditoFornecedor { get; set; }

        /// <summary>
        /// Número sequencial da forma de pagamento
        /// </summary>
        [PersistenceProperty("NumFormaPagto", PersistenceParameterType.Key)]
        public uint NumFormaPagto { get; set; }

        [PersistenceProperty("ValorPagto", PersistenceParameterType.Key)]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IdFormaPagto")]
        public uint? IdFormaPagto { get; set; }

        //RICARDO ALTERAÇÃO
        [PersistenceProperty("IdContaBanco")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IdTipoCartao")]
        public uint? IdTipoCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("FormaPagamento", DirectionParameter.InputOptional)]
        public string FormaPagamento { get; set; }

        [PersistenceProperty("ContaBanco", DirectionParameter.InputOptional)]
        public string ContaBanco { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string TipoCartao
        {
            get
            {
                if(IdTipoCartao.GetValueOrDefault(0) > 0)
                {
                    var tipoCartao = TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(IdTipoCartao.Value);

                    return tipoCartao != null ? tipoCartao.Descricao : "";
                }

                return "";
            }
        }

        #endregion
    }
}