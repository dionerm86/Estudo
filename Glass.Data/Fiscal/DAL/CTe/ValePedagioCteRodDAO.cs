using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ValePedagioCteRodDAO : BaseDAO<ValePedagioCteRod, ValePedagioCteRodDAO>
    {
        //private ValePedagioCteRodDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From vale_pedagio_cte_rod Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public ValePedagioCteRod GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new ValePedagioCteRod();
            }
        }

        public IList<ValePedagioCteRod> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<ValePedagioCteRod> GetValesPedagioCte(uint idCte)
        {
            return GetValesPedagioCte(null, idCte);
        }

        public List<ValePedagioCteRod> GetValesPedagioCte(GDASession session, uint idCte)
        {
            return objPersistence.LoadData(session, Sql(idCte, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

        public void Delete(GDASession session, uint idCte)
        {
            string sql = "delete from seguro_cte where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
