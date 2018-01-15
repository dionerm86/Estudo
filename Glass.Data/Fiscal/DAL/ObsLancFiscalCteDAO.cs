using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ObsLancFiscalCteDAO : BaseDAO<ObsLancFiscalCte, ObsLancFiscalCteDAO>
    {
        //private ObsLancFiscalCteDAO() { }

        private string Sql(uint idCte, bool selecionar)
        {
            string campos = selecionar ? "olfc.*, olf.descricao" : "count(*)";
            string sql = "select " + campos + @"
                from obs_lanc_fiscal_cte olfc
                    left join obs_lanc_fiscal olf on (olfc.idObsLancFiscal=olf.idObsLancFiscal)
                where 1";

            if (idCte > 0)
                sql += " and olfc.idCte=" + idCte;

            return sql;
        }

        public IList<ObsLancFiscalCte> GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCte, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCte, false));
        }

        public IList<ObsLancFiscalCte> GetByCte(uint idCte)
        {
            return objPersistence.LoadData(Sql(idCte, true)).ToList();
        }

        public string GetDescrByCte(uint idCte)
        {
            string retorno = "";
            foreach (ObsLancFiscalCte obs in objPersistence.LoadData(Sql(idCte, true)))
                retorno += obs.Descricao + ", ";

            return retorno.Trim(' ', ',');
        }
    }
}
