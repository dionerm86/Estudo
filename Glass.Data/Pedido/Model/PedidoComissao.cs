using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoComissaoDAO))]
    [PersistenceClass("pedido_comissao")]
    public class PedidoComissao
    {
        #region Propriedades

        [PersistenceProperty("IDPEDIDOCOMISSAO", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoComissao { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [PersistenceProperty("IDCOMISSIONADO", DirectionParameter.OnlyInsert)]
        public uint? IdComissionado { get; set; }

        [PersistenceProperty("IDINSTALADOR", DirectionParameter.OnlyInsert)]
        public uint? IdInstalador { get; set; }

        [PersistenceProperty("VALORPAGAR")]
        public decimal ValorPagar { get; set; }

        [PersistenceProperty("VALORPAGO")]
        public decimal ValorPago { get; set; }

        [PersistenceProperty("IDGERENTE")]
        public uint? IdGerente { get; set; }

        [PersistenceProperty("DATAALT")]
        public DateTime? DataAlt { get; set; }

        #endregion
    }
}