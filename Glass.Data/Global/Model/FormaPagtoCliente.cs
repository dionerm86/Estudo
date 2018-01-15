using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FormaPagtoClienteDAO))]
	[PersistenceClass("formapagto_cliente")]
	public class FormaPagtoCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCLIENTE", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int IdCliente { get; set; }

        [PersistenceProperty("IDFORMAPAGTO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(FormaPagto), "IdFormaPagto")]
        public int IdFormaPagto { get; set; }

        [PersistenceProperty("TIPOVENDA")]
        public int TipoVenda { get; set; }

        #endregion
    }
}