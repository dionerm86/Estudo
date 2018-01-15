using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.RelModel;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutosPedidoRptDAL : BaseDAO<ProdutosPedidoRpt, ProdutosPedidoRptDAL>
    {
        //public ProdutosPedidoRptDAL() { }

        /// <summary>
        /// Cria uma cópia da listagem de produtos de pedidos passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public ProdutosPedidoRpt[] CopiaLista(ProdutosPedido[] lstProdutosPedido)
        {
            List<uint> ambientes = new List<uint>();

            ProdutosPedidoRpt[] lstProdutosPedidoRpt = new ProdutosPedidoRpt[lstProdutosPedido.Length];
            for (int i = 0; i < lstProdutosPedido.Length; i++)
            {
                bool incluirQtdeAmbiente = lstProdutosPedido[i].IdAmbientePedido > 0 && 
                    !ambientes.Contains(lstProdutosPedido[i].IdAmbientePedido.Value);

                if (incluirQtdeAmbiente)
                    ambientes.Add(lstProdutosPedido[i].IdAmbientePedido.Value);

                lstProdutosPedidoRpt[i] = new ProdutosPedidoRpt(lstProdutosPedido[i], incluirQtdeAmbiente);
            }

            return lstProdutosPedidoRpt;
        }
    }
}
