using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ContabilistaDAO : BaseCadastroDAO<Contabilista, ContabilistaDAO>
    {
        //private ContabilistaDAO() { }

        public string Sql(uint idContabilista, bool selecionar)
        {
            string sql = @"
                Select " + (selecionar ? "c.*, cid.nomeCidade, cid.codIbgeCidade, cid.codIbgeUf" : "Count(*)") + @"
                From contabilista c
                    Inner Join cidade cid On (c.idCidade=cid.idCidade)
                Where 1";

            if (idContabilista > 0)
                sql += " And idContabilista=" + idContabilista;

            return sql;
        }

        public Contabilista GetElement(uint idContabilista)
        {
            return objPersistence.LoadOneData(Sql(idContabilista, true));
        }

        public IList<Contabilista> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }
    }
}
