using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class CobrancaDuplCteDAO : BaseDAO<CobrancaDuplCte, CobrancaDuplCteDAO>
    {
        //private CobrancaDuplCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From cobranca_dupl_cte Where 1";

            if (!selecionar)
                sql = "Select count(*) From cobranca_dupl_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public CobrancaDuplCte GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new CobrancaDuplCte();
            }
        }

        public CobrancaDuplCte[] GetList(uint idCte)
        {
            return GetList(null, idCte);
        }

        public CobrancaDuplCte[] GetList(GDASession session, uint idCte)
        {
            if (GetCount(session, idCte) > 0)
                return objPersistence.LoadData(session, Sql(idCte, true)).ToArray();

            return new CobrancaDuplCte[] { new CobrancaDuplCte() };
        }

        public IList<CobrancaDuplCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCte)
        {
            return GetCount(null, idCte);
        }

        public int GetCount(GDASession session, uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(idCte, false), null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

        public void Delete( uint idCte)
        {
            Delete(null, idCte);
        }

        public void Delete(GDASession sessao, uint idCte)
        {
            string sql = "delete from cobranca_dupl_cte where IDCTE= " + idCte;
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
