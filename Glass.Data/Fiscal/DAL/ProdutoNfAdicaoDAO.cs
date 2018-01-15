using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoNfAdicaoDAO : BaseDAO<ProdutoNfAdicao, ProdutoNfAdicaoDAO>
    {
        //private ProdutoNfAdicaoDAO() { }

        public ProdutoNfAdicao[] GetByProdNf(uint idProdNf)
        {
            var sql = @"select * from produto_nf_adicao
                where idProdNf=" + idProdNf + " order by numSeqAdicao asc";

            var retorno = objPersistence.LoadData(sql).ToList().ToArray();
            return retorno.Length > 0 ? retorno : new ProdutoNfAdicao[] { new ProdutoNfAdicao() };
        }
        
        #region Métodos sobrescritos

        public override uint Insert(ProdutoNfAdicao objInsert)
        {
            string sqlNumSeq = @"select coalesce(max(numSeqAdicao),0) + 1 from produto_nf_adicao
                where idProdNf=" + objInsert.IdProdNf;

            objInsert.NumSeqAdicao = ExecuteScalar<int>(sqlNumSeq);
            return base.Insert(objInsert);
        }

        #endregion
    }
}
