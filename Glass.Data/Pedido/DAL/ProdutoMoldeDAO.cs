using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoMoldeDAO : BaseDAO<ProdutoMolde, ProdutoMoldeDAO>
    {
        //private ProdutoMoldeDAO() { }

        #region Método de busca padrão

        private string Sql(uint idMolde, int numeroTabela, bool selecionar)
        {
            string campos = selecionar ? @"pm.*, ap.ambiente, p.idProd, p.descricao as descrProduto, p.codInterno as codInternoProduto, 
                pp.qtde as qtdeOriginal, pp.altura as alturaOriginal, pp.largura as larguraOriginal, 
                cast((select sum(qtde) from produto_molde where idProdPed=pm.idProdPed) as signed) as qtdeJaUtilizada" : "count(*)";

            string sql = @"
                select " + campos + @"
                from produto_molde pm
                    left join produtos_pedido pp on (pm.idProdPed=pp.idProdPed)
                    left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                    left join produto p on (pp.idProd=p.idProd)
                where 1";

            if (idMolde > 0)
                sql += " and pm.idMolde=" + idMolde;

            if (numeroTabela > 0)
                sql += " and pm.numTabela=" + numeroTabela;

            return sql;
        }

        /// <summary>
        /// Retorna os produtos de um molde.
        /// </summary>
        /// <param name="idMolde"></param>
        /// <returns></returns>
        public IList<ProdutoMolde> GetByMolde(uint idMolde, int numeroTabela, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idMolde, numeroTabela, true), sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Retorna o número de produtos de um molde.
        /// </summary>
        /// <param name="idMolde"></param>
        /// <returns></returns>
        public int GetByMoldeCount(uint idMolde, int numeroTabela)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idMolde, numeroTabela, false));
        }

        /// <summary>
        /// Retorna os produtos de um molde para o relatório.
        /// </summary>
        /// <param name="idMolde"></param>
        /// <returns></returns>
        public IList<ProdutoMolde> GetForRpt(uint idMolde, int numeroTabela)
        {
            return objPersistence.LoadData(Sql(idMolde, numeroTabela, true)).ToList();
        }

        #endregion

        #region Exclui os produtos de um molde

        /// <summary>
        /// Exclui os produtos de um molde.
        /// </summary>
        /// <param name="idMolde"></param>
        public void DeleteByMolde(uint idMolde)
        {
            objPersistence.ExecuteCommand("delete from produto_molde where idMolde=" + idMolde);
        }

        #endregion
    }
}
