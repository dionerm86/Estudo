using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutosPedidoEspelhoRptDAO : BaseDAO<ProdutosPedidoEspelhoRpt, ProdutosPedidoEspelhoRptDAO>
    {
        //private ProdutosPedidoEspelhoRptDAO() { }

        /// <summary>
        /// Cria uma cópia da listagem de produtos de pedidos espelho passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public ProdutosPedidoEspelhoRpt[] CopiaLista(ProdutosPedidoEspelho[] lstProdPed, ProdutosPedidoEspelhoRpt.TipoConstrutor tipo)
        {
            ProdutosPedidoEspelhoRpt[] lstProdPedRpt = new ProdutosPedidoEspelhoRpt[lstProdPed.Length];
            for (int i = 0; i < lstProdPed.Length; i++)
                lstProdPedRpt[i] = new ProdutosPedidoEspelhoRpt(lstProdPed[i], tipo);

            return lstProdPedRpt;
        }
    }
}
