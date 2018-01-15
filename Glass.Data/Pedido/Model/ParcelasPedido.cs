using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ParcelasPedidoDAO))]
	[PersistenceClass("parcelas_pedido")]
	public class ParcelasPedido
    {
        #region Propriedades

        [PersistenceProperty("NumParc", PersistenceParameterType.IdentityKey)]
        public int NumParc { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime? Data { get; set; }

        #endregion

        #region Propriedades de suporte

        public string DescrValor
        {
            get { return Valor.ToString("F2"); }
        }

        public decimal Desconto { get; set; }

        #endregion
    }
}