using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class SeguroCteDAO : BaseDAO<SeguroCte, SeguroCteDAO>
    {
        //private SeguroCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From seguro_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public SeguroCte GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public SeguroCte GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, true));
            }
            catch (GDA.GDAException)
            {
                return new SeguroCte();
            }
        }

        public IList<SeguroCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<SeguroCte> GetSegurosByIdCte(uint idCte)
        {
            return objPersistence.LoadData(Sql(idCte, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

       
        public override int Update(SeguroCte objUpdate)
        {
            return GDAOperations.Update(objUpdate);
        }

        public void Delete(uint idCte)
        {
            Delete(null, idCte);
        }

        public void Delete(GDASession sessao, uint idCte)
        {
            string sql = "delete from seguro_cte where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
