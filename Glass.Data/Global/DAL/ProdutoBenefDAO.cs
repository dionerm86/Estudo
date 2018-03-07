using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoBenefDAO : BaseDAO<ProdutoBenef, ProdutoBenefDAO>
    {
        //private ProdutoBenefDAO() { }

        public ProdutoBenef[] GetByProduto(uint idProd)
        {
            string sql = "select * from produto_benef where idProd=" + idProd;
            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        public ProdutoBenef[] GetByProdutos(string idsProds)
        {
            string sql = "select * from produto_benef where idProd IN(" + idsProds + ")";
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
            string sql = "delete from produto_benef where idProd=" + idProd;
            objPersistence.ExecuteCommand(session, sql);
        }

        public IList<ProdutoBenef> GetByNf(uint idNf)
        {
            var sql = @"
                SELECT pb.*, CONCAT(COALESCE(bc1.Descricao, ''), IF(COALESCE(bc1.Descricao, '') <> '', ' ', ''), bc.Descricao) as DescrBenef
                FROM produtos_nf pnf
	                INNER JOIN produto_benef pb ON (pnf.IdProd = pb.IdProd)
                    INNER JOIN benef_config bc ON (pb.IdBenefConfig = bc.IdBenefConfig)
                    LEFT JOIN benef_config bc1 ON (bc1.IdBenefConfig = bc.IdParent)
                WHERE idnf=" + idNf;

            return objPersistence.LoadData(sql).ToList();
        }
    }
}
