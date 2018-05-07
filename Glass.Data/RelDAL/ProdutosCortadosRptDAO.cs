using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutosCortadosRptDAO : BaseDAO<ProdutosCortadosRpt, ProdutosCortadosRptDAO>
    {
        /// <summary>
        /// Cria uma cópia da listagem de pedidos passada.
        /// </summary>
        /// <param name="lstPedido"></param>
        /// <returns></returns>
        public ProdutosCortadosRpt[] CopiaLista(ProdutosPedido[] lstProd)
        {
            var lstProdRpt = new ProdutosCortadosRpt[lstProd.Length];
            for (int i = 0; i < lstProdRpt.Length; i++)
                lstProdRpt[i] = new ProdutosCortadosRpt(lstProd[i]);

            return lstProdRpt;
        }
    }
}
