using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TextoImprPedidoDAO))]
	[PersistenceClass("texto_impr_pedido")]
	public class TextoImprPedido
    {
        #region Propriedades

        [PersistenceProperty("IDTEXTOIMPRPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdTextoImprPedido { get; set; }

        [Log("Título")]
        [PersistenceProperty("TITULO")]
        public string Titulo { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Buscar Sempre")]
        [PersistenceProperty("BUSCARSEMPRE")]
        public bool BuscarSempre { get; set; }
        
        #endregion
    }
}