using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PagtoTrocaDevolucaoDAO : BaseDAO<PagtoTrocaDevolucao, PagtoTrocaDevolucaoDAO>
    {
        //private PagtoTrocaDevolucaoDAO() { }

        private string Sql(uint idTrocaDevolucao, bool selecionar)
        {
            string campos = selecionar ? "ptd.*, fp.descricao as DescrFormaPagto" : "Count(*)";
            string sql = "select " + campos + " from pagto_troca_dev ptd " +
                "left join formapagto fp on (ptd.idFormaPagto=fp.idFormaPagto) " +
                "where 1";

            if (idTrocaDevolucao > 0)
                sql += " And idTrocaDevolucao=" + idTrocaDevolucao;

            return sql + " order by ptd.NumFormaPagto asc";
        }

        public IList<PagtoTrocaDevolucao> GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            return objPersistence.LoadData(Sql(idTrocaDevolucao, true)).ToList();
        }
    }
}
