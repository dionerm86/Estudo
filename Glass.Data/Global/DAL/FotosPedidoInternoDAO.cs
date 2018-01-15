using System.Linq;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public class FotosPedidoInternoDAO : BaseDAO<FotosPedidoInterno, FotosPedidoInternoDAO>
    {
        /// <summary>
        /// Retorna todas as fotos da sugestão.
        /// </summary>
        public FotosPedidoInterno[] ObterPeloPedidoInterno(int idPedidoInterno)
        {
            var sql = string.Format("SELECT * FROM fotos_pedido_interno WHERE IDPEDIDOINTERNO={0}", idPedidoInterno);
            return objPersistence.LoadData(sql).ToArray();
        }
    }
}
