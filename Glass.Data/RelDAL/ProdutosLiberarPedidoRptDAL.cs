using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutosLiberarPedidoRptDAL : BaseDAO<ProdutosLiberarPedidoRpt, ProdutosLiberarPedidoRptDAL>
    {
        //public ProdutosLiberarPedidoRptDAL() { }

        /// <summary>
        /// Cria uma cópia da listagem de pedidos passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public ProdutosLiberarPedidoRpt[] CopiaLista(ProdutosLiberarPedido[] lstProd, Pedido[] lstPedidos)
        {
            ProdutosLiberarPedidoRpt[] lstProdRpt = new ProdutosLiberarPedidoRpt[lstProd.Length];
            for (int i = 0; i < lstProdRpt.Length; i++)
            {
                lstProd[i].Pedido = lstPedidos.FirstOrDefault(f => f.IdPedido == lstProd[i].IdPedido);
                lstProdRpt[i] = new ProdutosLiberarPedidoRpt(lstProd[i]);
            }

            return lstProdRpt;
        }
    }
}
