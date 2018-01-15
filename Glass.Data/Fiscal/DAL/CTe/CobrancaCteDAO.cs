using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class CobrancaCteDAO : BaseDAO<CobrancaCte, CobrancaCteDAO>
    {
        //private CobrancaCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From cobranca_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public CobrancaCte GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public CobrancaCte GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, true));
            }
            catch
            {
                return new CobrancaCte();
            }
        }

        public IList<CobrancaCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion
    }
}
