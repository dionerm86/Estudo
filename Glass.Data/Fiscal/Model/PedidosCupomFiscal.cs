using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidosCupomFiscalDAO))]
    [PersistenceClass("pedidos_cupom_fiscal")]
    public class PedidosCupomFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDPEDIDOCUPOMFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoCFe { get; set; }

        [PersistenceProperty("IDCUPOMFISCAL")]
        public uint IdCupomFiscal { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        #endregion
    }
}