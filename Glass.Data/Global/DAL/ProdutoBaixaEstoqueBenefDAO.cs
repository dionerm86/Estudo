using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoBaixaEstoqueBenefDAO : BaseDAO<ProdutoBaixaEstoqueBenef, ProdutoBaixaEstoqueBenefDAO>
    {
        public ProdutoBaixaEstoqueBenef[] GetByProdutoBaixaEstoque(uint idProdBaixaEst)
        {
            string sql = "select * from produto_baixa_estoque_benef where idProdBaixaEst=" + idProdBaixaEst;
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public ProdutoBaixaEstoqueBenef[] GetByProdutos(string idsProdsBaixaEst)
        {
            string sql = "select * from produto_baixa_estoque_benef where idProdBaixaEst IN(" + idsProdsBaixaEst + ")";
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        /// <summary>
        /// Verifica se o produto possui beneficiamento
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public bool ProdutoPossuiBenef(uint idProd)
        {
            return ExecuteScalar<bool>($"SELECT COUNT(*)>0 FROM produto WHERE IdProd={ idProd }");
        }

        public void DeleteByProd(uint idProd)
        {
            DeleteByProd((GDASession)null, idProd);
        }

        public void DeleteByProd(GDASession session, uint idProd)
        {
            string sql = "delete from produto_baixa_estoque_benef where idProd=" + idProd;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
