using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutoPedidoProducaoRptDAL : BaseDAO<ProdutoPedidoProducaoRpt, ProdutoPedidoProducaoRptDAL>
    {
        //public ProdutoPedidoProducaoRptDAL() { }

        /// <summary>
        /// Cria uma cópia da listagem de pedidos passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public ProdutoPedidoProducaoRpt[] CopiaLista(ProdutoPedidoProducao[] lstProd)
        {
            ProdutoPedidoProducaoRpt[] lstProdRpt = new ProdutoPedidoProducaoRpt[lstProd.Length];
            for (int i = 0; i < lstProdRpt.Length; i++)
                lstProdRpt[i] = new ProdutoPedidoProducaoRpt(lstProd[i]);

            return lstProdRpt;
        }
    }
}
