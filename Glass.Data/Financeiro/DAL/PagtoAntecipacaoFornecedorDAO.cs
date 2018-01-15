using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoAntecipacaoFornecedorDAO : BaseDAO<PagtoAntecipacaoFornecedor, PagtoAntecipacaoFornecedorDAO>
    {
        //private PagtoAntecipacaoFornecedorDAO() { }

        private string Sql(uint idPagtoAntecipFornec, bool selecionar)
        {
            string campos = selecionar ? "paf.*, fp.descricao as FormaPagto" : "Count(*)";

            string sql = @"
                Select " + campos + @" 
                From pagto_antecipacao_fornecedor paf 
                    Left Join formapagto fp on (paf.idFormaPagto=fp.idFormaPagto) 
                Where 1";

            if (idPagtoAntecipFornec > 0)
                sql += " And paf.idAntecipFornec=" + idPagtoAntecipFornec;

            return sql + " order by paf.NumFormaPagto asc";
        }

        public PagtoAntecipacaoFornecedor[] GetByAntecipFornec(uint idAntecipFornec)
        {
            return GetByAntecipFornec(null, idAntecipFornec);
        }

        public PagtoAntecipacaoFornecedor[] GetByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            return objPersistence.LoadData(session, Sql(idAntecipFornec, true)).ToArray();
        }

        public void DeleteByAntecipFornec(uint idAntecipFornec)
        {
            DeleteByAntecipFornec(null, idAntecipFornec);
        }

        public void DeleteByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            objPersistence.ExecuteCommand(session, "delete from pagto_antecipacao_fornecedor where idAntecipFornec=" + idAntecipFornec);
        }
    }
}
