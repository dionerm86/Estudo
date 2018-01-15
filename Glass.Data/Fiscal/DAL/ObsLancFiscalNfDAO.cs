using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ObsLancFiscalNfDAO : BaseDAO<ObsLancFiscalNf, ObsLancFiscalNfDAO>
    {
        //private ObsLancFiscalNfDAO() { }

        private string Sql(uint idNf, bool selecionar)
        {
            string campos = selecionar ? "olfn.*, olf.descricao" : "count(*)";
            string sql = "select " + campos + @"
                from obs_lanc_fiscal_nf olfn
                    left join obs_lanc_fiscal olf on (olfn.idObsLancFiscal=olf.idObsLancFiscal)
                where 1";

            if (idNf > 0)
                sql += " and olfn.idNf=" + idNf;

            return sql;
        }

        public IList<ObsLancFiscalNf> GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idNf, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, false));
        }

        public IList<ObsLancFiscalNf> GetByNf(uint idNf)
        {
            return objPersistence.LoadData(Sql(idNf, true)).ToList();
        }

        public string GetDescrByNf(uint idNf)
        {
            string retorno = "";
            foreach (ObsLancFiscalNf obs in objPersistence.LoadData(Sql(idNf, true)))
                retorno += obs.Descricao + ", ";

            return retorno.Trim(' ', ',');
        }
    }
}
