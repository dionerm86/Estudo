using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class MotoristaCteRodDAO : BaseDAO<MotoristaCteRod, MotoristaCteRodDAO>
    {
        //private MotoristaCteRodDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From motorista_cte_rod Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public MotoristaCteRod GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new MotoristaCteRod();
            }
        }

        public IList<MotoristaCteRod> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<MotoristaCteRod> GetMotoristasIdCte(uint idCte)
        {
            return GetMotoristasIdCte(null, idCte);
        }

        public List<MotoristaCteRod> GetMotoristasIdCte(GDASession session, uint idCte)
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
            string sql = "delete from motorista_cte_rod where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
