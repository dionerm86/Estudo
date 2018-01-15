using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TextoPedidoDAO))]
	[PersistenceClass("texto_pedido")]
	public class TextoPedido
    {
        #region Propriedades

        [PersistenceProperty("IDTEXTOPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdTextoPedido { get; set; }

        [PersistenceProperty("IDTEXTOIMPRPEDIDO")]
        public uint IdTextoImprPedido { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }
        
        [PersistenceProperty("TITULO", DirectionParameter.InputOptional)]
        public string Titulo { get; set; }
        
        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        #endregion
    }
}