using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using System.Linq;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class PedidoRptDAL : BaseDAO<PedidoRpt, PedidoRptDAL>
    {
        //public PedidoRptDAL() { }

        /// <summary>
        /// Cria uma cópia da listagem de pedidos passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public PedidoRpt[] CopiaLista(Pedido[] lstPedido, PedidoRpt.TipoConstrutor tipo, bool mostrarDescontoTotal, LoginUsuario login)
        {
            return lstPedido.Where(x =>
            {
                x.Login = login;
                return true;
            }).Select(x => new PedidoRpt(x, tipo, mostrarDescontoTotal)).ToArray();
        }
    }
}
